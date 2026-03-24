using static TicketWebApp.Models.DTOs.TicketAssignmentDtos;

namespace TicketWebApp.Interfaces
{
    public interface IAutoAssignmentService
    {
        Task<TicketAssignmentResponseDto?> AutoAssignAsync(long ticketId, long assignedByUserId, TicketAutoAssignRequestDto? request = null);
    }
}
