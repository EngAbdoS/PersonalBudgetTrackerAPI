using PersonalBudgetTrackerAPI.Identity;
using PersonalBudgetTrackerAPI.Models.Auth;

namespace PersonalBudgetTrackerAPI.Services.Interfaces
{
    public interface IJwtService
    {
         Task<AuthResponse> GenerateJWT(ApplicationUser user);
    }
}
