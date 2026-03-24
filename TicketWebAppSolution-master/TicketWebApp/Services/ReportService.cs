using Microsoft.EntityFrameworkCore;
using TicketWebApp.Interfaces;
using TicketWebApp.Models;
using TicketWebApp.Models.DTOs;

namespace TicketWebApp.Services
{
    public class ReportService : IReportService
    {
        private readonly IRepository<long, Ticket> _ticketRepo;
        private readonly IRepository<int, Status> _statusRepo;
        private readonly IRepository<int, Priority> _priorityRepo;

        public ReportService(
            IRepository<long, Ticket> ticketRepo,
            IRepository<int, Status> statusRepo,
            IRepository<int, Priority> priorityRepo)
        {
            _ticketRepo = ticketRepo;
            _statusRepo = statusRepo;
            _priorityRepo = priorityRepo;
        }

        public async Task<TicketSummaryDto> GetTicketSummaryAsync(long currentUserId, bool isAdmin, bool isAgent, bool isEmployee)
        {
            // Ensure required lookups exist
            var statusConfigured = await _statusRepo.GetQueryable().AnyAsync();
            if (!statusConfigured)
                throw new InvalidOperationException("No statuses are configured.");

            var prioritiesConfigured = await _priorityRepo.GetQueryable().AnyAsync();
            if (!prioritiesConfigured)
                throw new InvalidOperationException("No priorities are configured.");

            // Base query
            var q = _ticketRepo.GetQueryable()
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .AsQueryable();

            // Scope by role: Employees only see their own tickets
            if (isEmployee && !isAdmin && !isAgent)
            {
                if (currentUserId <= 0)
                    throw new ArgumentException("Invalid current user id for employee scope.", nameof(currentUserId));

                q = q.Where(t => t.CreatedByUserId == currentUserId);
            }

            // Materialize for aggregations
            var items = await q
                .Select(t => new
                {
                    t.Id,
                    StatusName = t.Status != null ? t.Status.Name : null,
                    PriorityName = t.Priority != null ? t.Priority.Name : null,
                    t.CurrentAssigneeUserId,
                    t.CreatedByUserId,
                    t.DueAt
                })
                .ToListAsync();

            var now = DateTime.UtcNow;

            bool IsStatus(string? s, string target) =>
                !string.IsNullOrWhiteSpace(s) && s.Trim().Equals(target, StringComparison.OrdinalIgnoreCase);

            // Status buckets
            var openCount = items.Count(t => IsStatus(t.StatusName, "Open") || IsStatus(t.StatusName, "New"));
            var inProgressCount = items.Count(t => IsStatus(t.StatusName, "In Progress"));
            var resolvedCount = items.Count(t => IsStatus(t.StatusName, "Resolved"));
            var closedCount = items.Count(t => IsStatus(t.StatusName, "Closed"));

            // Priority buckets
            int highPriority = items.Count(t => string.Equals(t.PriorityName, "High", StringComparison.OrdinalIgnoreCase));
            int mediumPriority = items.Count(t => string.Equals(t.PriorityName, "Medium", StringComparison.OrdinalIgnoreCase));
            int lowPriority = items.Count(t => string.Equals(t.PriorityName, "Low", StringComparison.OrdinalIgnoreCase));

            // Overdue: DueAt < now and not terminal (Closed/Resolved)
            int overdue = items.Count(t =>
                t.DueAt.HasValue
                && t.DueAt.Value < now
                && !IsStatus(t.StatusName, "Closed")
                && !IsStatus(t.StatusName, "Resolved"));

            // Assigned to me (for Admin/Agent dashboards)
            int assignedToMe = (isAdmin || isAgent)
                ? items.Count(t => t.CurrentAssigneeUserId.HasValue && t.CurrentAssigneeUserId.Value == currentUserId)
                : 0;

            return new TicketSummaryDto
            {
                Total = items.Count,
                Open = openCount,
                InProgress = inProgressCount,
                Resolved = resolvedCount,
                Closed = closedCount,
                HighPriority = highPriority,
                MediumPriority = mediumPriority,
                LowPriority = lowPriority,
                AssignedToMe = assignedToMe,
                Overdue = overdue
            };
        }
    }
}