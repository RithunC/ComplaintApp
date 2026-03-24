using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TicketWebApp.Contexts;
using TicketWebApp.Interfaces;
using TicketWebApp.Models.DTOs;
using static TicketWebApp.Models.DTOs.CommentDtos;

namespace TicketWebApp.Controllers
{
    [ApiController]
    [Route("api/comments")]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _comments;
        private readonly ComplaintContext _ctx;

        public CommentsController(ICommentService comments, ComplaintContext ctx)
        {
            _comments = comments;
            _ctx = ctx;
        }

        //Returns null if claim missing/invalid.
        private long? CurrentUserId()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return long.TryParse(id, out var uid) ? uid : (long?)null;
        }

        private static bool IsStaff(string roleName)
            => string.Equals(roleName, "Admin", StringComparison.OrdinalIgnoreCase)
               || string.Equals(roleName, "Agent", StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Add a comment to a ticket. isInternal is allowed only for Agent/Admin.
        /// Comments are disabled when the ticket is in a closed state (Resolved/Closed).
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Add(
            [FromBody] CommentCreateDto dto,
            [FromHeader(Name = "X-User-Id")] long? xUserId = null)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Who is posting? (dev: header; prod: claims)
            var postedByUserIdNullable = xUserId ?? CurrentUserId();
            if (postedByUserIdNullable is null) return Unauthorized();
            var postedByUserId = postedByUserIdNullable.Value;

            // Validate user exists and get their role from DB (authoritative)
            var user = await _ctx.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == postedByUserId && u.IsActive);
            if (user == null)
                return Unauthorized("User not found or inactive.");

            var roleName = user.Role?.Name ?? "User";
            var isStaff = IsStaff(roleName);

            // Ensure ticket exists and read IsClosedState in one round-trip
            var ticketRow = await (from t in _ctx.Tickets
                                   join s in _ctx.Statuses on t.StatusId equals s.Id
                                   where t.Id == dto.TicketId
                                   select new
                                   {
                                       Ticket = t,
                                       IsClosed = s.IsClosedState
                                   }).FirstOrDefaultAsync();

            if (ticketRow == null) return NotFound("Ticket not found.");

            // ❌ Block commenting for closed tickets (Resolved/Closed)
            if (ticketRow.IsClosed)
                return Conflict("Comments are disabled because the ticket is in a closed state.");

            // Authorization: only creator, current assignee, or staff can comment
            var isCreator = ticketRow.Ticket.CreatedByUserId == postedByUserId;
            var isAssignee = ticketRow.Ticket.CurrentAssigneeUserId == postedByUserId;
            if (!(isCreator || isAssignee || isStaff))
                return Forbid("You are not allowed to comment on this ticket.");

            // Automatically force employees' comments to be public
            if (!isStaff)
            {
                dto.IsInternal = false;
            }

            try
            {
                var res = await _comments.AddAsync(postedByUserId, dto);
                return Ok(res);
            }
            catch (InvalidOperationException ex)
            {
                // In case service also rejects (defense-in-depth)
                return Conflict(ex.Message);
            }
        }

        /// <summary>
        /// Get comments for a ticket. Internal comments are hidden for non-staff.
        /// </summary>
        [HttpGet("ticket/{ticketId:long}")]
        public async Task<IActionResult> GetByTicket(
            long ticketId,
            [FromHeader(Name = "X-User-Id")] long? xUserId = null)
        {
            // Identify caller
            var requesterIdNullable = xUserId ?? CurrentUserId();
            if (requesterIdNullable is null) return Unauthorized();
            var requesterId = requesterIdNullable.Value;

            // Get user & role (authoritative)
            var user = await _ctx.Users.Include(u => u.Role)
                                       .FirstOrDefaultAsync(u => u.Id == requesterId && u.IsActive);
            var roleName = user?.Role?.Name ?? "User";
            var isStaff = user != null && IsStaff(roleName);

            // Optional: also allow ticket creator and current assignee to see all public comments
            var ticket = await _ctx.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);
            if (ticket == null) return NotFound("Ticket not found.");

            var isCreator = ticket.CreatedByUserId == requesterId;
            var isAssignee = ticket.CurrentAssigneeUserId == requesterId;

            var list = await _comments.GetByTicketAsync(ticketId);

            // Hide internal comments for non-staff
            if (!isStaff)
            {
                list = list.Where(c => !c.IsInternal).ToList();
            }

            // (Optional) If you also want to restrict even public comments to only participants, uncomment:
            // if (!(isCreator || isAssignee || isStaff))
            //     return Forbid("You are not allowed to view comments for this ticket.");

            return Ok(list);
        }

        /// <summary>
        /// Edit a comment's body.
        /// Rules are enforced in service:
        /// - Only staff can edit internal comments.
        /// - Non-staff can edit only their own comments.
        /// - Comments are disabled for closed tickets.
        /// </summary>
        [HttpPatch("{id:long}")]
        public async Task<IActionResult> Edit(
            long id,
            [FromBody] CommentEditDto dto,
            [FromHeader(Name = "X-User-Id")] long? xUserId = null)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Body))
            {
                ModelState.AddModelError("body", "Body is required.");
                return ValidationProblem(ModelState);
            }

            var editorIdNullable = xUserId ?? CurrentUserId();
            if (editorIdNullable is null) return Unauthorized();
            var editorId = editorIdNullable.Value;

            try
            {
                var res = await _comments.EditAsync(editorId, id, dto.Body.Trim());
                if (res == null) return NotFound("Comment not found.");
                return Ok(res);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        /// <summary>
        /// Delete a comment.
        /// Rules are enforced in service:
        /// - Only staff can delete internal comments.
        /// - Non-staff can delete only their own comments.
        /// - Comments are disabled for closed tickets.
        /// </summary>
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(
            long id,
            [FromHeader(Name = "X-User-Id")] long? xUserId = null)
        {
            var deleterIdNullable = xUserId ?? CurrentUserId();
            if (deleterIdNullable is null) return Unauthorized();
            var deleterId = deleterIdNullable.Value;

            try
            {
                var ok = await _comments.DeleteAsync(deleterId, id);
                if (!ok) return NotFound("Comment not found.");
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}