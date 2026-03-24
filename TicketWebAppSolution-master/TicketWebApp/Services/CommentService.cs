using Microsoft.EntityFrameworkCore;
using TicketWebApp.Interfaces;
using TicketWebApp.Models;
using static TicketWebApp.Models.DTOs.CommentDtos;

namespace TicketWebApp.Services
{
    public class CommentService : ICommentService
    {
        private readonly IRepository<long, Comment> _commentRepo;
        private readonly IRepository<long, Ticket> _ticketRepo;
        private readonly IRepository<long, User> _userRepo;

        public CommentService(
            IRepository<long, Comment> commentRepo,
            IRepository<long, Ticket> ticketRepo,
            IRepository<long, User> userRepo)
        {
            _commentRepo = commentRepo;
            _ticketRepo = ticketRepo;
            _userRepo = userRepo;
        }

        private static bool IsStaff(string? roleName)
            => string.Equals(roleName, "Admin", StringComparison.OrdinalIgnoreCase)
               || string.Equals(roleName, "Agent", StringComparison.OrdinalIgnoreCase);

        public async Task<CommentResponseDto> AddAsync(long postedByUserId, CommentCreateDto dto)
        {
            var ticketStatusRow = await _ticketRepo.GetQueryable()
                .Where(t => t.Id == dto.TicketId)
                .Select(t => new { t.Id, IsClosedState = t.Status!.IsClosedState })
                .FirstOrDefaultAsync();

            if (ticketStatusRow == null)
                throw new InvalidOperationException("Ticket not found.");

            if (ticketStatusRow.IsClosedState)
                throw new InvalidOperationException("Comments are disabled for closed tickets.");

            var comment = new Comment
            {
                TicketId = dto.TicketId,
                PostedByUserId = postedByUserId,
                Body = dto.Body,
                IsInternal = dto.IsInternal,
                CreatedAt = DateTime.UtcNow
            };

            await _commentRepo.Add(comment);

            var by = await _userRepo.Get(postedByUserId);

            return new CommentResponseDto
            {
                Id = comment.Id,
                TicketId = comment.TicketId,
                Body = comment.Body,
                IsInternal = comment.IsInternal,
                PostedByUserId = postedByUserId,
                PostedByName = by?.DisplayName ?? "",
                CreatedAt = comment.CreatedAt
            };
        }

        public async Task<IReadOnlyList<CommentResponseDto>> GetByTicketAsync(long ticketId)
        {
            var list = await _commentRepo.GetQueryable()
                .Include(c => c.PostedBy)
                .Where(c => c.TicketId == ticketId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return list.Select(c => new CommentResponseDto
            {
                Id = c.Id,
                TicketId = c.TicketId,
                Body = c.Body,
                IsInternal = c.IsInternal,
                PostedByUserId = c.PostedByUserId,
                PostedByName = c.PostedBy?.DisplayName ?? "",
                CreatedAt = c.CreatedAt
            }).ToList();
        }

        public async Task<CommentResponseDto?> EditAsync(long editorUserId, long commentId, string newBody)
        {
            var row = await _commentRepo.GetQueryable()
                .Include(c => c.PostedBy)
                .Include(c => c.Ticket)!.ThenInclude(t => t!.Status)
                .Where(c => c.Id == commentId)
                .Select(c => new
                {
                    Comment = c,
                    IsClosed = c.Ticket!.Status!.IsClosedState
                }).FirstOrDefaultAsync();

            if (row == null) return null;
            if (row.IsClosed)
                throw new InvalidOperationException("Comments are disabled for closed tickets.");

            var editor = await _userRepo.GetQueryable()
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == editorUserId && u.IsActive);

            if (editor == null)
                throw new UnauthorizedAccessException("User not found or inactive.");

            var isStaff = IsStaff(editor.Role?.Name);

            if (row.Comment.IsInternal && !isStaff)
                throw new UnauthorizedAccessException("Only staff can edit internal comments.");

            if (!isStaff && row.Comment.PostedByUserId != editorUserId)
                throw new UnauthorizedAccessException("You can edit only your own comments.");

            row.Comment.Body = newBody;
            await _commentRepo.Update(row.Comment.Id, row.Comment);

            return new CommentResponseDto
            {
                Id = row.Comment.Id,
                TicketId = row.Comment.TicketId,
                Body = row.Comment.Body,
                IsInternal = row.Comment.IsInternal,
                PostedByUserId = row.Comment.PostedByUserId,
                PostedByName = row.Comment.PostedBy?.DisplayName ?? "",
                CreatedAt = row.Comment.CreatedAt
            };
        }

        public async Task<bool> DeleteAsync(long deleterUserId, long commentId)
        {
            var row = await _commentRepo.GetQueryable()
                .Include(c => c.Ticket)!.ThenInclude(t => t!.Status)
                .Where(c => c.Id == commentId)
                .Select(c => new { Comment = c, IsClosed = c.Ticket!.Status!.IsClosedState })
                .FirstOrDefaultAsync();

            if (row == null) return false;
            if (row.IsClosed)
                throw new InvalidOperationException("Comments are disabled for closed tickets.");

            var deleter = await _userRepo.GetQueryable()
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == deleterUserId && u.IsActive);

            if (deleter == null)
                throw new UnauthorizedAccessException("User not found or inactive.");

            var isStaff = IsStaff(deleter.Role?.Name);

            if (row.Comment.IsInternal && !isStaff)
                throw new UnauthorizedAccessException("Only staff can delete internal comments.");

            if (!isStaff && row.Comment.PostedByUserId != deleterUserId)
                throw new UnauthorizedAccessException("You can delete only your own comments.");

            await _commentRepo.Delete(row.Comment.Id);
            return true;
        }
    }
}