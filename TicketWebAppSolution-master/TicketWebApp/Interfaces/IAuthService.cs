// File: Interfaces/IAuthService.cs
using System.Threading.Tasks;
// Aliases to nested DTOs
using AuthReq = TicketWebApp.Models.DTOs.AuthDtos.CheckUserRequestDto;
using AuthRes = TicketWebApp.Models.DTOs.AuthDtos.CheckUserResponseDto;
using RegReq = TicketWebApp.Models.DTOs.AuthDtos.RegisterUserRequestDto;
using RegRes = TicketWebApp.Models.DTOs.AuthDtos.RegisterUserResponseDto;

// NEW: Aliases for forgot/reset password
using ForgotReq = TicketWebApp.Models.DTOs.AuthDtos.ForgotPasswordRequestDto;
using ForgotRes = TicketWebApp.Models.DTOs.AuthDtos.ForgotPasswordResponseDto;
using ResetReq = TicketWebApp.Models.DTOs.AuthDtos.ResetPasswordRequestDto;
using BasicRes = TicketWebApp.Models.DTOs.AuthDtos.BasicResponseDto;

namespace TicketWebApp.Interfaces
{
    public interface IAuthService
    {
        Task<AuthRes> LoginAsync(AuthReq request);
        Task<RegRes> RegisterAsync(RegReq request);


        // NEW:
        Task<ForgotRes> ForgotPasswordAsync(ForgotReq request);
        Task<BasicRes> ResetPasswordAsync(ResetReq request);

    }
}