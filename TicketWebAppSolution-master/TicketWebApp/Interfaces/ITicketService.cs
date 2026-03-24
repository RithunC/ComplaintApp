using TicketWebApp.Models.Common;
using TicketWebApp.Models.DTOs;
using static TicketWebApp.Models.DTOs.TicketAssignmentDtos;
using static TicketWebApp.Models.DTOs.TicketDtos;

namespace TicketWebApp.Interfaces
{
    public interface ITicketService
    {

        Task<TicketResponseDto> CreateAsync(long createdByUserId, TicketCreateDto dto);
        Task<TicketResponseDto?> GetAsync(long id);
        Task<PagedResult<TicketListItemDto>> QueryAsync(TicketQueryDto query);
        Task<TicketResponseDto?> UpdateAsync(long id, TicketUpdateDto dto);
        Task<bool> UpdateStatusAsync(long id, long changedByUserId, TicketStatusUpdateDto dto);
        Task<TicketAssignmentResponseDto?> AssignAsync(long id, long assignedByUserId, TicketAssignRequestDto dto);
        Task<IReadOnlyList<TicketStatusHistoryDto>> GetStatusHistoryAsync(long ticketId);

    }
}
