using TicketWebApp.Models.DTOs;

namespace TicketWebApp.Interfaces
{
    public interface ILookupService
    {
        Task<IReadOnlyList<DepartmentDto>> GetDepartmentsAsync();
        Task<IReadOnlyList<RoleDto>> GetRolesAsync();
        Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync();
        Task<IReadOnlyList<PriorityDto>> GetPrioritiesAsync();
        Task<IReadOnlyList<StatusDto>> GetStatusesAsync();

    }
}
