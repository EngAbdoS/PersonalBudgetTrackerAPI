using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PersonalBudgetTrackerAPI.DTOs.Auth;
using PersonalBudgetTrackerAPI.Identity;
using PersonalBudgetTrackerAPI.Models.Auth;
using PersonalBudgetTrackerAPI.Services.Interfaces;
using System.Security.Claims;

namespace PersonalBudgetTrackerAPI.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtService _jwtService;           
        private readonly ITokenStore _tokenStore;

        public AuthController(UserManager<ApplicationUser> userManager, IJwtService jwtService, ITokenStore tokenStore)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _tokenStore = tokenStore;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            var emailExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null || emailExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponse { Message = "User or email already exists!" });

            ApplicationUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                FullName = model.FullName,
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponse { Message = "User creation failed! Please check user details and try again." });

            var tokenResponse = await _jwtService.GenerateJWT(user);

            await _tokenStore.StoreRefreshToken(user.Id.ToString(),tokenResponse.RefreshToken);


            return Ok(tokenResponse);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);

            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var tokenResponse = await _jwtService.GenerateJWT(user);

                await _tokenStore.StoreRefreshToken(user.Id.ToString(), tokenResponse.RefreshToken);

                return Ok(tokenResponse);
            }
            return Unauthorized();
        }


        [HttpPost("refresh")]
        public async Task<IActionResult> GenerateAccessToken([FromBody] TokenModelDto tokenModel)
        {
            if (tokenModel == null)
            {
                return BadRequest("Invalid client request");
            }

            ClaimsPrincipal? principal = _jwtService.GetPrincipalFromJWTToken(tokenModel.Token);
            if (principal == null)
            { return BadRequest("invalid jwt access token "); }


            string email = principal.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
            ApplicationUser? user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("Invalid access token");
            }
            var isValid = await _tokenStore.ValidateRefreshToken(user.Id, tokenModel.RefreshToken??"");


            if (!isValid)
            {
                return BadRequest("Invalid refresh token");
            }

            var tokenResponse = await _jwtService.GenerateJWT(user);

            await _tokenStore.StoreRefreshToken(user.Id.ToString(), tokenResponse.RefreshToken);


            return Ok(tokenResponse);

        }


    }
}
