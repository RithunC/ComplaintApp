using Microsoft.EntityFrameworkCore;
using TicketWebApp.Interfaces;
using TicketWebApp.Models;
using TicketWebApp.Models.DTOs;

namespace TicketWebApp.Services
{
    public class LookupService : ILookupService
    {
        private readonly IRepository<int, Department> _depRepo;
        private readonly IRepository<int, Role> _roleRepo;
        private readonly IRepository<int, Category> _categoryRepo;
        private readonly IRepository<int, Priority> _priorityRepo;
        private readonly IRepository<int, Status> _statusRepo;

        public LookupService(
            IRepository<int, Department> depRepo,
            IRepository<int, Role> roleRepo,
            IRepository<int, Category> categoryRepo,
            IRepository<int, Priority> priorityRepo,
            IRepository<int, Status> statusRepo)
        {
            _depRepo = depRepo;
            _roleRepo = roleRepo;
            _categoryRepo = categoryRepo;
            _priorityRepo = priorityRepo;
            _statusRepo = statusRepo;
        }

        public async Task<IReadOnlyList<DepartmentDto>> GetDepartmentsAsync()
        {
            var list = await _depRepo.GetQueryable()
                .OrderBy(d => d.Name)
                .Select(d => new DepartmentDto { Id = d.Id, Name = d.Name })
                .ToListAsync();

            if (list.Count == 0)
                throw new InvalidOperationException("No departments are configured.");

            return list;
        }

        public async Task<IReadOnlyList<RoleDto>> GetRolesAsync()
        {
            var list = await _roleRepo.GetQueryable()
                .OrderBy(r => r.Name)
                .Select(r => new RoleDto { Id = r.Id, Name = r.Name })
                .ToListAsync();

            if (list.Count == 0)
                throw new InvalidOperationException("No roles are configured.");

            return list;
        }

        public async Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync()
        {
            var list = await _categoryRepo.GetQueryable()
                .OrderBy(c => c.Name)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ParentCategoryId = c.ParentCategoryId,
                    ParentCategoryName = c.ParentCategory != null ? c.ParentCategory.Name : null
                })
                .ToListAsync();

            if (list.Count == 0)
                throw new InvalidOperationException("No categories are configured.");

            return list;
        }

        public async Task<IReadOnlyList<PriorityDto>> GetPrioritiesAsync()
        {
            var list = await _priorityRepo.GetQueryable()
                .OrderBy(p => p.Rank)
                .Select(p => new PriorityDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Rank = p.Rank,
                    ColorHex = p.ColorHex
                })
                .ToListAsync();

            if (list.Count == 0)
                throw new InvalidOperationException("No priorities are configured.");

            return list;
        }

        public async Task<IReadOnlyList<StatusDto>> GetStatusesAsync()
        {
            var list = await _statusRepo.GetQueryable()
                .OrderBy(s => s.Name)
                .Select(s => new StatusDto { Id = s.Id, Name = s.Name, IsClosedState = s.IsClosedState })
                .ToListAsync();

            if (list.Count == 0)
                throw new InvalidOperationException("No statuses are configured.");

            return list;
        }
    }
}
