// File: Services/AuthService.cs
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TicketWebApp.Contexts;
using TicketWebApp.Interfaces;

// Aliases to your nested DTOs:
using AuthReq = TicketWebApp.Models.DTOs.AuthDtos.CheckUserRequestDto;
using AuthRes = TicketWebApp.Models.DTOs.AuthDtos.CheckUserResponseDto;
using TokenPayload = TicketWebApp.Models.DTOs.AuthDtos.TokenPayloadDto;
using RegReq = TicketWebApp.Models.DTOs.AuthDtos.RegisterUserRequestDto;
using RegRes = TicketWebApp.Models.DTOs.AuthDtos.RegisterUserResponseDto;


// NEW: Aliases for forgot/reset password
using ForgotReq = TicketWebApp.Models.DTOs.AuthDtos.ForgotPasswordRequestDto;
using ForgotRes = TicketWebApp.Models.DTOs.AuthDtos.ForgotPasswordResponseDto;
using ResetReq = TicketWebApp.Models.DTOs.AuthDtos.ResetPasswordRequestDto;
using BasicRes = TicketWebApp.Models.DTOs.AuthDtos.BasicResponseDto;


namespace TicketWebApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly ComplaintContext _ctx;
        private readonly IPasswordService _passwords;
        private readonly ITokenService _tokens;

        public AuthService(ComplaintContext ctx, IPasswordService passwords, ITokenService tokens)
        {
            _ctx = ctx; _passwords = passwords; _tokens = tokens;
        }

        // File: Services/AuthService.cs

        public async Task<AuthRes> LoginAsync(AuthReq request)
        {
            var user = await _ctx.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserName == request.UserName && u.IsActive);

            if (user == null)
                return new AuthRes { Token = string.Empty }; // <- changed

            var ok = _passwords.VerifyPassword(
                request.Password,
                Convert.ToBase64String(user.PasswordHash),
                user.PasswordSalt);

            if (!ok)
                return new AuthRes { Token = string.Empty }; // <- changed

            var token = _tokens.CreateToken(new TokenPayload
            {
                Username = user.UserName,
                Role = user.Role?.Name ?? "",
                UserId = user.Id
            });

            return new AuthRes
            {
                Token = token // <- token-only
            };
        }

        public async Task<RegRes> RegisterAsync(RegReq request)
        {
            // Basic validations
            if (string.IsNullOrWhiteSpace(request.UserName) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.DisplayName) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.RoleName))
            {
                return new RegRes { Success = false, Message = "UserName, Email, DisplayName, Password, RoleName are required." };
            }

            // Uniqueness
            if (await _ctx.Users.AnyAsync(u => u.UserName == request.UserName))
                return new RegRes { Success = false, Message = "Username already exists" };

            if (await _ctx.Users.AnyAsync(u => u.Email == request.Email))
                return new RegRes { Success = false, Message = "Email already exists" };

            // Resolve Role by name
            var role = await _ctx.Roles.FirstOrDefaultAsync(r => r.Name == request.RoleName);
            if (role == null)
                return new RegRes { Success = false, Message = "Invalid RoleName" };

            // Resolve Department by name (optional)
            int? departmentId = null;
            if (!string.IsNullOrWhiteSpace(request.DepartmentName))
            {
                var dep = await _ctx.Departments.FirstOrDefaultAsync(d => d.Name == request.DepartmentName);
                if (dep == null)
                    return new RegRes { Success = false, Message = "Invalid DepartmentName" };
                departmentId = dep.Id;
            }

            // Hash password (returns Base64 string + salt bytes)
            byte[] salt;
            var base64Hash = _passwords.HashPassword(request.Password, null, out salt);

            var user = new TicketWebApp.Models.User
            {
                UserName = request.UserName,
                Email = request.Email,
                DisplayName = request.DisplayName,

                // store bytes; login compares using Convert.ToBase64String(user.PasswordHash)
                PasswordHash = Convert.FromBase64String(base64Hash),
                PasswordSalt = salt,

                RoleId = role.Id,
                DepartmentId = departmentId,
                IsActive = true,
                IsEmailVerified = false,
                CreatedAt = DateTime.UtcNow
            };

            _ctx.Users.Add(user);
            await _ctx.SaveChangesAsync();

            var token = _tokens.CreateToken(new TokenPayload
            {
                Username = user.UserName,
                Role = role.Name,
                UserId = user.Id
            });

            return new RegRes
            {
                Success = true,
                UserId = user.Id,
                UserName = user.UserName,
                Role = role.Name,
                Token = token,
                Message = "User registered successfully"
            };
        }

        // NEW: Forgot Password - generate short-lived reset token
        public async Task<ForgotRes> ForgotPasswordAsync(ForgotReq request)
        {
            if (string.IsNullOrWhiteSpace(request.UserNameOrEmail))
                return new ForgotRes { Success = false, Message = "UserNameOrEmail is required." };

            var user = await _ctx.Users
                .FirstOrDefaultAsync(u =>
                    u.IsActive &&
                    (u.UserName == request.UserNameOrEmail || u.Email == request.UserNameOrEmail));

            if (user == null)
                return new ForgotRes { Success = false, Message = "User not found." };

            var resetToken = _tokens.CreatePasswordResetToken(user.Id, user.UserName);

            // In production, email/SMS this token. For now, return it for client-side flow.
            return new ForgotRes
            {
                Success = true,
                Message = "Password reset token generated.",
                ResetToken = resetToken
            };
        }

        // NEW: Reset Password - validate token and write new password
        public async Task<BasicRes> ResetPasswordAsync(ResetReq request)
        {
            if (string.IsNullOrWhiteSpace(request.Token))
                return new BasicRes { Success = false, Message = "Token is required." };
            if (string.IsNullOrWhiteSpace(request.NewPassword))
                return new BasicRes { Success = false, Message = "NewPassword is required." };

            if (!_tokens.TryValidatePasswordResetToken(request.Token, out var userId, out var username))
                return new BasicRes { Success = false, Message = "Invalid or expired token." };

            var user = await _ctx.Users.FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);
            if (user == null)
                return new BasicRes { Success = false, Message = "User not found or inactive." };

            // Hash new password using the exact same logic you already use
            byte[] salt;
            var base64Hash = _passwords.HashPassword(request.NewPassword, null, out salt);

            user.PasswordHash = Convert.FromBase64String(base64Hash);
            user.PasswordSalt = salt;

            await _ctx.SaveChangesAsync();

            return new BasicRes { Success = true, Message = "Password has been reset successfully." };
        }

    }
}