using static TicketWebApp.Models.DTOs.ErrorLogDtos;

namespace TicketWebApp.Interfaces
{
    public interface IErrorLogService
    {
        Task<ErrorLogResponseDto> CreateAsync(ErrorLogCreateDto dto);
        Task<IReadOnlyList<ErrorLogResponseDto>> GetRecentAsync(int take = 100);

    }
}
