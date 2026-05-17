using PersonalBudgetTrackerAPI.DTOs.Auth;
using PersonalBudgetTrackerAPI.Identity;
using System.Security.Claims;

namespace PersonalBudgetTrackerAPI.Services.Interfaces.Auth
{
    public interface IJwtService
    {
         Task<AuthResponseDTO> GenerateJWT(ApplicationUser user);
         ClaimsPrincipal? GetPrincipalFromJWTToken(string? token);

    }
}
