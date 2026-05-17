using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalBudgetTrackerAPI.Common;
using PersonalBudgetTrackerAPI.DTOs.Auth;
using PersonalBudgetTrackerAPI.Services.Interfaces.Auth;
using PersonalBudgetTrackerAPI.Common.Exceptions;

namespace PersonalBudgetTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var result = await _authService.RegisterAsync(model);
            return Ok(ApiResponse<AuthResponseDTO>.Ok(result, "User registered successfully."));
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var result = await _authService.LoginAsync(model);
            return Ok(ApiResponse<AuthResponseDTO>.Ok(result, "Login successful."));
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> GenerateAccessToken([FromBody] TokenModelDto tokenModel)
        {
            var result = await _authService.GenerateAccessTokenAsync(tokenModel);
            return Ok(ApiResponse<AuthResponseDTO>.Ok(result, "Token refreshed successfully."));
        }

        [Authorize]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto model)
        {
            await _authService.UpdateProfileAsync(model);
            return Ok(ApiResponse<bool>.Ok(true, "Profile updated successfully."));
        }

        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            await _authService.ChangePasswordAsync(model);
            return Ok(ApiResponse<bool>.Ok(true, "Password changed successfully."));
        }
    }
}
