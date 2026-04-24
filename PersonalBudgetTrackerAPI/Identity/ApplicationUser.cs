using Microsoft.AspNetCore.Identity;

namespace PersonalBudgetTrackerAPI.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
