using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketWebApp.Interfaces;

namespace TicketWebApp.Controllers
{
    [ApiController]
    [Route("api/lookups")]
    [Authorize]
    public class LookupsController : ControllerBase
    {
        private readonly ILookupService _lookups;

        public LookupsController(ILookupService lookups)
        {
            _lookups = lookups;
        }

        [HttpGet("departments")]
        public async Task<IActionResult> GetDepartments() => Ok(await _lookups.GetDepartmentsAsync());

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles() => Ok(await _lookups.GetRolesAsync());

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories() => Ok(await _lookups.GetCategoriesAsync());

        [HttpGet("priorities")]
        public async Task<IActionResult> GetPriorities() => Ok(await _lookups.GetPrioritiesAsync());

        [HttpGet("statuses")]
        public async Task<IActionResult> GetStatuses() => Ok(await _lookups.GetStatusesAsync());
    }
}