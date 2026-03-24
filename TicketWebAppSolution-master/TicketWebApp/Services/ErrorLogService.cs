using Microsoft.EntityFrameworkCore;
using TicketWebApp.Interfaces;
using TicketWebApp.Models;
using static TicketWebApp.Models.DTOs.ErrorLogDtos;

namespace TicketWebApp.Services
{
    public class ErrorLogService : IErrorLogService
    {
        private readonly IRepository<int, ErrorLog> _errorRepo;

        public ErrorLogService(IRepository<int, ErrorLog> errorRepo)
        {
            _errorRepo = errorRepo;
        }

        public async Task<ErrorLogResponseDto> CreateAsync(ErrorLogCreateDto dto)
        {
            var log = new ErrorLog
            {
                ErrorMessage = dto.ErrorMessage,
                ErrorNumber = dto.ErrorNumber,
                CreatedAt = DateTime.UtcNow
            };
            await _errorRepo.Add(log);

            return new ErrorLogResponseDto
            {
                ErrorId = log.ErrorId,
                ErrorMessage = log.ErrorMessage,
                ErrorNumber = log.ErrorNumber,
                CreatedAt = log.CreatedAt
            };
        }

        public async Task<IReadOnlyList<ErrorLogResponseDto>> GetRecentAsync(int take = 100)
        {
            if (take <= 0 || take > 1000) take = 100;

            var list = await _errorRepo.GetQueryable()
                .OrderByDescending(e => e.CreatedAt)
                .Take(take)
                .ToListAsync();

            return list.Select(e => new ErrorLogResponseDto
            {
                ErrorId = e.ErrorId,
                ErrorMessage = e.ErrorMessage,
                ErrorNumber = e.ErrorNumber,
                CreatedAt = e.CreatedAt
            }).ToList();
        }
    }
}