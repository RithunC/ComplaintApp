using Microsoft.EntityFrameworkCore;
using TicketWebApp.Interfaces;
using TicketWebApp.Models;
using static TicketWebApp.Models.DTOs.TicketAssignmentDtos;

namespace TicketWebApp.Services
{
    public class AutoAssignmentService : IAutoAssignmentService
    {
        private readonly ITicketService _ticketService;
        private readonly IRepository<long, Ticket> _ticketRepo;
        private readonly IRepository<long, User> _userRepo;

        public AutoAssignmentService(
            ITicketService ticketService,
            IRepository<long, Ticket> ticketRepo,
            IRepository<long, User> userRepo)
        {
            _ticketService = ticketService;  //Assign injected service to local variable
            _ticketRepo = ticketRepo;
            _userRepo = userRepo;
        }

        public async Task<TicketAssignmentResponseDto?> AutoAssignAsync(
            long ticketId,
            long assignedByUserId,
            TicketAutoAssignRequestDto? request = null)
        {
            // Validate the assigner exists (safe guard; helps audit and avoids ghost actions)
            var assignedByExists = await _userRepo.GetQueryable()
                .AnyAsync(u => u.Id == assignedByUserId);
            if (!assignedByExists)
                throw new KeyNotFoundException("AssignedBy user not found.");

            var ticket = await _ticketRepo.GetQueryable()
                .Include(t => t.Status)
                .FirstOrDefaultAsync(t => t.Id == ticketId);
            if (ticket == null) return null;

            var depId = request?.DepartmentId ?? ticket.DepartmentId;

            // Find active agents, optionally filtered by department
            var agents = await _userRepo.GetQueryable()
                .Include(u => u.Role)
                .Where(u => u.IsActive
                            && u.Role != null && u.Role.Name == "Agent"
                            && (!depId.HasValue || u.DepartmentId == depId))
                .Select(u => new { u.Id })
                .ToListAsync();

            if (agents.Count == 0) return null;

            var agentIds = agents.Select(a => a.Id).ToList();

            // Calculate current open load per agent (non-closed tickets)
            var openCounts = await _ticketRepo.GetQueryable()
                .Include(t => t.Status)
                .Where(t => t.CurrentAssigneeUserId != null
                            && agentIds.Contains(t.CurrentAssigneeUserId.Value)
                            && t.Status != null && !t.Status.IsClosedState)
                .GroupBy(t => t.CurrentAssigneeUserId!.Value)
                .Select(g => new { UserId = g.Key, OpenCount = g.Count() })
                .ToListAsync();

            var loadDict = openCounts.ToDictionary(x => x.UserId, x => x.OpenCount); //faster lookup

            // Choose least-loaded agent (tie-break by user id)
            var chosenId = agents
                .Select(a => new { a.Id, Open = loadDict.TryGetValue(a.Id, out var c) ? c : 0 }) //if agents has tickets use count,else 0
                .OrderBy(x => x.Open).ThenBy(x => x.Id) //sort least tickets
                .First().Id; //pick best agent

            return await _ticketService.AssignAsync(ticketId, assignedByUserId, new TicketAssignRequestDto
            {
                AssignedToUserId = chosenId,
                Note = request?.Note ?? "Auto-assigned to least loaded agent"
            });
        }
    }
}