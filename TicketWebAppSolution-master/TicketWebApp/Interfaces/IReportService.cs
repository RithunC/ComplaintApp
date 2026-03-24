using TicketWebApp.Models.DTOs;

namespace TicketWebApp.Interfaces
{
    public interface IReportService
    {
        /// <summary>
        /// Returns ticket summary respecting role:
        /// Admin/Agent -> global; Employee -> only tickets created by them.
        /// </summary>
        Task<TicketSummaryDto> GetTicketSummaryAsync(long currentUserId, bool isAdmin, bool isAgent, bool isEmployee);
    }
}