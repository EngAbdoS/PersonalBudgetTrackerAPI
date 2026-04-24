using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PersonalBudgetTrackerAPI.DTOs.Auth;

namespace PersonalBudgetTrackerAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpPost("create-role")]
        public async Task<IActionResult> CreateRole([FromBody] RoleDto model)
        {
            var roleExists = await _roleManager.RoleExistsAsync(model.RoleName);
            if (!roleExists)
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(model.RoleName));

                if (roleResult.Succeeded)
                {
                    return Ok( $"Role {model.RoleName} added successfully" );
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, $"Issue adding the new {model.RoleName} role");
                }
            }
            return BadRequest( "Role already exists" );
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            var roleExists = await _roleManager.RoleExistsAsync(model.RoleName);
            if (!roleExists)
            {
                return BadRequest("Role not found" );
            }

            var result = await _userManager.AddToRoleAsync(user, model.RoleName);
            if (result.Succeeded)
            {
                return Ok($"Role {model.RoleName} assigned to user {model.Username} successfully" );
            }

            return BadRequest("Failed to assign role");
        }

    }
}
