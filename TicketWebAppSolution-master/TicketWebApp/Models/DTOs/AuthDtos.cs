
namespace TicketWebApp.Models.DTOs
{
    public class AuthDtos
    {
        public class CheckUserRequestDto
        {
            public string UserName { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class CheckUserResponseDto
        {

            public string Token { get; set; } = string.Empty; // token-only respons

        }

        public class TokenPayloadDto
        {
            public string Username { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
            public long UserId { get; set; }
        }

        // Register by names (no numeric IDs)
        public class RegisterUserRequestDto
        {
            public string UserName { get; set; } = string.Empty;   // unique
            public string Email { get; set; } = string.Empty;      // unique
            public string DisplayName { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;

            public string RoleName { get; set; } = string.Empty;   // REQUIRED: Admin/Agent/Employee
            public string? DepartmentName { get; set; }            // OPTIONAL: e.g., IT Support
        }

        public class RegisterUserResponseDto
        {
            public bool Success { get; set; }
            public long? UserId { get; set; }
            public string? UserName { get; set; }
            public string? Role { get; set; }
            public string? Token { get; set; }
            public string? Message { get; set; }
        }

        // NEW: Forgot/Reset Password DTOs (no new DB models)
        public class ForgotPasswordRequestDto
        {
            // Accept either username or email to start the reset flow
            public string UserNameOrEmail { get; set; } = string.Empty;
        }

        public class ForgotPasswordResponseDto
        {
            public bool Success { get; set; }
            public string? Message { get; set; }
            // For now we return the reset token; you can email it instead
            public string? ResetToken { get; set; }
        }

        public class ResetPasswordRequestDto
        {
            public string Token { get; set; } = string.Empty;
            public string NewPassword { get; set; } = string.Empty;
        }

        public class BasicResponseDto
        {
            public bool Success { get; set; }
            public string? Message { get; set; }
        }

    }
}