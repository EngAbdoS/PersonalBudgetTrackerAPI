using PersonalBudgetTrackerAPI.Identity;
using PersonalBudgetTrackerAPI.Models.Auth;
using System.Security.Claims;

namespace PersonalBudgetTrackerAPI.Services.Interfaces
{
    public interface IJwtService
    {
         Task<AuthResponse> GenerateJWT(ApplicationUser user);
         ClaimsPrincipal? GetPrincipalFromJWTToken(string? token);

    }
}
