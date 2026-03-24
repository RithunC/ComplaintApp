using static TicketWebApp.Models.DTOs.CommentDtos;

namespace TicketWebApp.Interfaces
{
    public interface ICommentService
    {
        Task<CommentResponseDto> AddAsync(long postedByUserId, CommentCreateDto dto);
        Task<IReadOnlyList<CommentResponseDto>> GetByTicketAsync(long ticketId);
        Task<CommentResponseDto?> EditAsync(long editorUserId, long commentId, string newBody);
        Task<bool> DeleteAsync(long deleterUserId, long commentId);


    }
}
