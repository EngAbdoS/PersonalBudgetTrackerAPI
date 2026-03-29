using Microsoft.AspNetCore.Identity;

namespace PersonalBudgetTrackerAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
