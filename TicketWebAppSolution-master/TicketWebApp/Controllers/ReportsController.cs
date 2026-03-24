using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketWebApp.Interfaces;

namespace TicketWebApp.Controllers
{
    [ApiController]
    [Route("api/reports")]
    [Authorize] // All endpoints require authentication
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reports;

        public ReportsController(IReportService reports)
        {
            _reports = reports;
        }

        // Removed hardcoded fallback. Returns null if claim missing/invalid.
        private long? CurrentUserId()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return long.TryParse(id, out var uid) ? uid : (long?)null;
        }

        // GET api/reports/tickets/summary
        // Admin/Agent -> global; Employee -> scoped to own tickets
        [HttpGet("tickets/summary")]
        [Authorize(Roles = "Admin,Agent,Employee")]
        public async Task<IActionResult> GetTicketSummary()
        {
            var currentUserId = CurrentUserId();
            if (currentUserId is null) return Unauthorized();

            var isAdmin = User.IsInRole("Admin");
            var isAgent = User.IsInRole("Agent");
            var isEmployee = User.IsInRole("Employee");

            var summary = await _reports.GetTicketSummaryAsync(
                currentUserId.Value,
                isAdmin,
                isAgent,
                isEmployee
            );

            return Ok(summary);
        }
    }
}