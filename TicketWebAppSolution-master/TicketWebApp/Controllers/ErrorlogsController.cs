using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketWebApp.Interfaces;

namespace TicketWebApp.Controllers
{
    [ApiController]
    [Route("api/errorlogs")]
    [Authorize(Roles = "Admin")]
    public class ErrorLogsController : ControllerBase
    {
        private readonly IErrorLogService _errors;

        public ErrorLogsController(IErrorLogService errors)
        {
            _errors = errors;
        }

        // GET api/errorlogs/recent?take=100
        [HttpGet("recent")]
        public async Task<IActionResult> GetRecent([FromQuery] int take = 100)
        {
            if (take <= 0 || take > 1000) take = 100;
            var data = await _errors.GetRecentAsync(take);
            return Ok(data);
        }
    }
}