using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketWebApp.Interfaces;
using TicketWebApp.Models.DTOs;

namespace TicketWebApp.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _users;

        public UsersController(IUserService users)
        {
            _users = users;
        }

        // GET api/users/{id}
        [HttpGet("{id:long}")]
        [Authorize]
        public async Task<IActionResult> Get(long id)
        {
            var u = await _users.GetAsync(id);
            return u == null ? NotFound() : Ok(u);
        }

        // GET api/users/agents?departmentId=1
        [HttpGet("agents")]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> GetAgents([FromQuery] int? departmentId = null)
        {
            var list = await _users.GetAgentsAsync(departmentId);
            return Ok(list);
        }


        // === NEW: GET api/users/me ===
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!long.TryParse(idClaim, out var myId))
                return Unauthorized();

            var u = await _users.GetAsync(myId);
            return u == null ? NotFound() : Ok(u);
        }

        // === NEW: PUT api/users/me ===
        [HttpPut("me")]
        [Authorize]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileDto model)
        {
            if (model == null)
                return BadRequest("No data provided.");

            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!long.TryParse(idClaim, out var myId))
                return Unauthorized();

            var updated = await _users.UpdateProfileAsync(myId, model);
            return updated == null ? NotFound() : Ok(updated);
        }
    }
}

    