using Microsoft.EntityFrameworkCore;
using TicketWebApp.Interfaces;
using TicketWebApp.Models;
using TicketWebApp.Models.DTOs;

namespace TicketWebApp.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<long, User> _userRepo;

        public UserService(IRepository<long, User> userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<UserLiteDto?> GetAsync(long id)
        {
            var u = await _userRepo.GetQueryable()
                .Include(x => x.Role)
                .Include(x => x.Department)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (u == null)
                throw new Exception("User not found");

            return new UserLiteDto
            {
                Id = u.Id,
                DisplayName = u.DisplayName,
                UserName = u.UserName,
                RoleName = u.Role?.Name ?? "",
                DepartmentId = u.DepartmentId,
                DepartmentName = u.Department?.Name,
                IsActive = u.IsActive
            };
        }

        public async Task<IReadOnlyList<UserLiteDto>> GetAgentsAsync(int? departmentId = null)
        {
            var query = _userRepo.GetQueryable()
                .Include(u => u.Role)
                .Include(u => u.Department)
                .Where(u => u.IsActive && u.Role!.Name == "Agent");

            if (departmentId.HasValue)
                query = query.Where(u => u.DepartmentId == departmentId);

            var list = await query
                .OrderBy(u => u.DisplayName)
                .Select(u => new UserLiteDto
                {
                    Id = u.Id,
                    DisplayName = u.DisplayName,
                    UserName = u.UserName,
                    RoleName = u.Role!.Name,
                    DepartmentId = u.DepartmentId,
                    DepartmentName = u.Department != null ? u.Department.Name : null,
                    IsActive = u.IsActive
                })
                .ToListAsync();

            if (list.Count == 0)
                throw new Exception("No agents found");

            return list;
        }

        public async Task<UserLiteDto?> UpdateProfileAsync(long userId, UpdateProfileDto model)
        {
            var u = await _userRepo.GetQueryable()
                .Include(x => x.Role)
                .Include(x => x.Department)
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (u == null)
                throw new Exception("User not found");

            // Allowed fields only
            if (model.DisplayName != null)
                u.DisplayName = model.DisplayName;

            if (model.Phone != null || (model.Phone == null && u.Phone != null))
                u.Phone = model.Phone;

            var saved = await _userRepo.Update(userId, u);
            var result = saved ?? u;

            return new UserLiteDto
            {
                Id = result.Id,
                DisplayName = result.DisplayName,
                UserName = result.UserName,
                RoleName = result.Role?.Name ?? "",
                DepartmentId = result.DepartmentId,
                DepartmentName = result.Department?.Name,
                IsActive = result.IsActive
            };
        }
    }
}