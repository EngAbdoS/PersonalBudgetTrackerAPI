using PersonalBudgetTrackerAPI.DTOs.Auth;

namespace PersonalBudgetTrackerAPI.Services.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> RegisterAsync(RegisterDto model);
        Task<AuthResponseDTO> LoginAsync(LoginDto model);
        Task<AuthResponseDTO> GenerateAccessTokenAsync(TokenModelDto tokenModel);
        Task<bool> UpdateProfileAsync(UpdateProfileDto model);
        Task<bool> ChangePasswordAsync(ChangePasswordDto model);
    }
}
