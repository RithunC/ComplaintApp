using TicketWebApp.Models.DTOs;


namespace TicketWebApp.Interfaces
{
    public interface IAttachmentService
    {
        Task<AttachmentResponseDto> UploadAsync(long ticketId, long uploadedByUserId, IFormFile file);
        Task<IReadOnlyList<AttachmentResponseDto>> GetByTicketAsync(long ticketId);

        Task<AttachmentDownloadResult?> GetDownloadAsync(long attachmentId, long requestingUserId);
        Task<bool> DeleteAsync(long attachmentId, long requestingUserId);


    }
}
