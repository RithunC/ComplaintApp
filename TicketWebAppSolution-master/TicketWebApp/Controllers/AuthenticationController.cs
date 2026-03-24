// File: Controllers/AuthenticationController.cs
using Microsoft.AspNetCore.Mvc;
using TicketWebApp.Interfaces;
// Aliases for nested DTOs
using AuthReq = TicketWebApp.Models.DTOs.AuthDtos.CheckUserRequestDto;
using RegReq = TicketWebApp.Models.DTOs.AuthDtos.RegisterUserRequestDto;

// NEW: Aliases for forgot/reset password
using ForgotReq = TicketWebApp.Models.DTOs.AuthDtos.ForgotPasswordRequestDto;
using ResetReq = TicketWebApp.Models.DTOs.AuthDtos.ResetPasswordRequestDto;

namespace TicketWebApp.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthenticationController(IAuthService auth)
        {
            _auth = auth;
        }
       

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthReq dto)
        {
            var result = await _auth.LoginAsync(dto);

            if (string.IsNullOrEmpty(result.Token))
                return Unauthorized("Invalid username or password");

            // result now has only { token: "..." }
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegReq dto)
        {
            var result = await _auth.RegisterAsync(dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        // NEW: Forgot Password (returns a short-lived reset token)
        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotReq dto)
        {
            var result = await _auth.ForgotPasswordAsync(dto);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        // NEW: Reset Password using token
        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetReq dto)
        {
            var result = await _auth.ResetPasswordAsync(dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

    }
}