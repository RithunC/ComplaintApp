using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketWebApp.Interfaces;
using TicketWebApp.Models.DTOs;
using static TicketWebApp.Models.DTOs.TicketAssignmentDtos;
using static TicketWebApp.Models.DTOs.TicketDtos;

namespace TicketWebApp.Controllers
{
    [Route("api/tickets")]
    [ApiController]
    [Authorize] // All endpoints require authentication
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _tickets;
        private readonly IAutoAssignmentService _autoAssign;

        public TicketsController(ITicketService tickets, IAutoAssignmentService autoAssign)
        {
            _tickets = tickets;
            _autoAssign = autoAssign;
        }

        // Returns null if claim missing/invalid.
        private long? CurrentUserId()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return long.TryParse(id, out var uid) ? uid : (long?)null;
        }

        // POST api/tickets
        // Only Employees (requesters) can create tickets.
        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Create(
            [FromBody] TicketCreateDto dto,
            [FromHeader(Name = "X-User-Id")] long? xUserId = null
        )
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var currentUserId = xUserId ?? CurrentUserId();
            if (currentUserId is null) return Unauthorized();

            var created = await _tickets.CreateAsync(currentUserId.Value, dto);

            // Always auto-assign on create (least-loaded agent)
            await _autoAssign.AutoAssignAsync(created.Id, currentUserId.Value, new TicketAutoAssignRequestDto
            {
                DepartmentId = created.DepartmentId,
                CategoryId = created.CategoryId,
                Note = "Auto-assign on ticket create"
            });

            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        // GET api/tickets/{id}
        // Any authenticated user can view
        [HttpGet("{id:long}")]
        public async Task<IActionResult> Get(long id)
        {
            var t = await _tickets.GetAsync(id);
            return t == null ? NotFound() : Ok(t);
        }



        // POST api/tickets/query
        // Any authenticated user can query (you can filter per-role inside service)
        [HttpPost("query")]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> Query([FromBody] TicketQueryDto query)
        {
            var result = await _tickets.QueryAsync(query);
            return Ok(result);
        }

        // PATCH api/tickets/{id}
        // Employees can update ONLY their own tickets (ownership enforced).
        // Admin/Agent can update any ticket.
        [HttpPatch("{id:long}")]
        [Authorize(Roles = "Employee,Agent,Admin")]
        public async Task<IActionResult> Update(long id, [FromBody] TicketUpdateDto dto)
        {
            var existing = await _tickets.GetAsync(id);
            if (existing == null) return NotFound();

            var currentUserId = CurrentUserId();
            if (currentUserId is null) return Unauthorized();

            var isEmployee = User.IsInRole("Employee");
            var isAgentOrAdmin = User.IsInRole("Agent") || User.IsInRole("Admin");

            if (isEmployee)
            {
                if (existing.CreatedByUserId != currentUserId.Value)
                    return Forbid();

                // Restrict fields employees can change:
                // Allow: PriorityId, Title, Description, DueAt
                dto.DepartmentId = null;
                dto.CategoryId = null;
            }

            var updated = await _tickets.UpdateAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }

        // POST api/tickets/{id}/assign
        // Manual assignment allowed only for Admin/Agent. Employees are blocked.
        [HttpPost("{id:long}/assign")]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> Assign(
            long id,
            [FromBody] TicketAssignRequestDto dto,
            [FromHeader(Name = "X-User-Id")] long? xUserId = null
        )
        {
            var currentUserId = xUserId ?? CurrentUserId();
            if (currentUserId is null) return Unauthorized();

            var res = await _tickets.AssignAsync(id, currentUserId.Value, dto);
            return res == null ? NotFound() : Ok(res);
        }

        // POST api/tickets/{id}/auto-assign
        // Triggerable by Admin/Agent (system flow). Employees must not call this.
        [HttpPost("{id:long}/autoAssign")]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> AutoAssign(
            long id,
            [FromBody] TicketAutoAssignRequestDto dto,
            [FromHeader(Name = "X-User-Id")] long? xUserId = null
        )
        {
            var currentUserId = xUserId ?? CurrentUserId();
            if (currentUserId is null) return Unauthorized();

            var res = await _autoAssign.AutoAssignAsync(id, currentUserId.Value, dto);
            return res == null ? NotFound("No eligible agent found") : Ok(res);
        }

        // POST api/tickets/{id}/status
        // Only Admin/Agent can move statuses.
        [HttpPost("{id:long}/status")]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> UpdateStatus(
            long id,
            [FromBody] TicketStatusUpdateDto dto,
            [FromHeader(Name = "X-User-Id")] long? xUserId = null
        )
        {
            var currentUserId = xUserId ?? CurrentUserId();
            if (currentUserId is null) return Unauthorized();

            var ok = await _tickets.UpdateStatusAsync(id, currentUserId.Value, dto);
            return ok ? Ok() : NotFound();
        }

        // GET api/tickets/{id}/history
        // Authorize: Admin/Agent -> can view any
        //            Employee    -> can view only their own ticket
        [HttpGet("{id:long}/history")]
        [Authorize(Roles = "Admin,Agent,Employee")]
        public async Task<IActionResult> GetHistory(long id)
        {
            // First, ensure the ticket exists (and to read who owns it)
            var ticket = await _tickets.GetAsync(id);
            if (ticket == null)
                return NotFound();

            var currentUserId = CurrentUserId();
            if (currentUserId is null) return Unauthorized();

            var isAdmin = User.IsInRole("Admin");
            var isAgent = User.IsInRole("Agent");
            var isEmployee = User.IsInRole("Employee");

            // Admin/Agent can view any ticket history
            if (!isAdmin && !isAgent)
            {
                // Employees can view history only for tickets they created
                if (isEmployee && ticket.CreatedByUserId != currentUserId.Value)
                    return Forbid();
            }

            var history = await _tickets.GetStatusHistoryAsync(id);
            return Ok(history);
        }
    }
}