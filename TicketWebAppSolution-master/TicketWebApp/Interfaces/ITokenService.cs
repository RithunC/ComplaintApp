// Alias for your nested DTO
using TokenPayload = TicketWebApp.Models.DTOs.AuthDtos.TokenPayloadDto;

namespace TicketWebApp.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(TokenPayload payload);

        // NEW: Token helpers specific to password reset
        string CreatePasswordResetToken(long userId, string username);
        bool TryValidatePasswordResetToken(string token, out long userId, out string username);

    }
}
