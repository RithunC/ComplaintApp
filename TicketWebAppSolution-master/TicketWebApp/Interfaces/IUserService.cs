using TicketWebApp.Models.DTOs;

namespace TicketWebApp.Interfaces
{
    public interface IUserService
    {
        Task<UserLiteDto?> GetAsync(long id);
        Task<IReadOnlyList<UserLiteDto>> GetAgentsAsync(int? departmentId = null);
        Task<UserLiteDto?> UpdateProfileAsync(long userId, UpdateProfileDto model);

    }
}
