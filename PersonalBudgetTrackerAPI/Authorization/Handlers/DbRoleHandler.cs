using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using PersonalBudgetTrackerAPI.Authorization.Requirements;
using PersonalBudgetTrackerAPI.Identity;

namespace PersonalBudgetTrackerAPI.Authorization.Handlers
{
    public class DbRoleHandler : AuthorizationHandler<DbRoleRequirement>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public DbRoleHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }


      protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        DbRoleRequirement requirement)
    {
        var user = await _userManager.GetUserAsync(context.User);

        if (user == null)
            return;

            if (await _userManager.IsInRoleAsync(user, requirement.Role))
        {
            context.Succeed(requirement);
        }
    }
    }
}
