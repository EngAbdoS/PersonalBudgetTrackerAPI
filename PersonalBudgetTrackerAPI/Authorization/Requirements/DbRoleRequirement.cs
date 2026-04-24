using Microsoft.AspNetCore.Authorization;

namespace PersonalBudgetTrackerAPI.Authorization.Requirements
{
    public class DbRoleRequirement : IAuthorizationRequirement
    {

        public string Role { get; }

        public DbRoleRequirement(string role)
        {
            Role = role;
        }

    }
}
