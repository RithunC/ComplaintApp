using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TicketWebApp.Interfaces;
// Alias for nested payload:
using TokenPayload = TicketWebApp.Models.DTOs.AuthDtos.TokenPayloadDto;

namespace TicketWebApp.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration configuration)
        {
            var secret = configuration["Keys:Jwt"] ?? throw new InvalidOperationException("Jwt key missing");
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        }

        public string CreateToken(TokenPayload payload)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, payload.Username),
                new Claim(ClaimTypes.Role, payload.Role),
                new Claim(ClaimTypes.NameIdentifier, payload.UserId.ToString())
            };
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = creds
            };
            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }

        // NEW: Issue a short-lived password reset token (15 minutes)
        public string CreatePasswordResetToken(long userId, string username)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim("purpose", "pwdreset")
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = creds
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }

        // NEW: Validate password reset token and extract user info
        public bool TryValidatePasswordResetToken(string token, out long userId, out string username)
        {
            userId = 0;
            username = string.Empty;

            var handler = new JwtSecurityTokenHandler();
            try
            {
                var parameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _key,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };

                var principal = handler.ValidateToken(token, parameters, out var validatedToken);

                // Ensure purpose claim exists and matches
                var purpose = principal.FindFirst("purpose")?.Value;
                if (!string.Equals(purpose, "pwdreset", StringComparison.Ordinal))
                    return false;

                var idClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var nameClaim = principal.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrWhiteSpace(idClaim) || !long.TryParse(idClaim, out userId))
                    return false;

                username = nameClaim ?? string.Empty;
                return true;
            }
            catch
            {
                return false;
            }
        }

        }
    }