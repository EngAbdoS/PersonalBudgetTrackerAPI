using Microsoft.AspNetCore.Identity;
using PersonalBudgetTrackerAPI.Common.Exceptions;
using PersonalBudgetTrackerAPI.DTOs.Auth;
using PersonalBudgetTrackerAPI.Identity;
using PersonalBudgetTrackerAPI.Services.Interfaces.Auth;
using System.Security.Claims;

namespace PersonalBudgetTrackerAPI.Services.Implementations.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtService _jwtService;
        private readonly ITokenStore _tokenStore;
        private readonly ICurrentUserService _currentUserService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IJwtService jwtService,
            ITokenStore tokenStore,
            ICurrentUserService currentUserService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _tokenStore = tokenStore;
            _currentUserService = currentUserService;
        }

        public async Task<AuthResponseDTO> RegisterAsync(RegisterDto model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            var emailExists = await _userManager.FindByEmailAsync(model.Email);

            if (userExists != null || emailExists != null)
                throw new BadRequestException("Username or email is already taken.");

            var user = new ApplicationUser
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                LanguageCode = model.LanguageCode,
                CurrencyCode = model.CurrencyCode,
                TimeZone = model.TimeZone,
                IsDarkMode = model.IsDarkMode,
                PushNotificationsEnabled = model.PushNotificationsEnabled
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                throw new BadRequestException($"User creation failed: {FormatIdentityErrors(result)}");

            return await IssueAndStoreTokensAsync(user);
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginDto model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                throw new UnauthorizedException("Invalid username or password.");

            return await IssueAndStoreTokensAsync(user);
        }

        public async Task<AuthResponseDTO> GenerateAccessTokenAsync(TokenModelDto tokenModel)
        {
            if (tokenModel is null)
                throw new BadRequestException("Invalid client request.");

            var principal = _jwtService.GetPrincipalFromJWTToken(tokenModel.Token);
            if (principal is null)
                throw new UnauthorizedException("Invalid access token.");

            var email = principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrWhiteSpace(email))
                throw new UnauthorizedException("Invalid access token.");

            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                throw new UnauthorizedException("Invalid access token.");

            var isValid = await _tokenStore.ValidateRefreshToken(user.Id, tokenModel.RefreshToken ?? "");
            if (!isValid)
                throw new UnauthorizedException("Invalid or expired refresh token.");

            return await IssueAndStoreTokensAsync(user);
        }

        public async Task<bool> UpdateProfileAsync(UpdateProfileDto model)
        {
            var user = await GetAuthenticatedUserAsync();

            if (model.FullName is not null)
                user.FullName = model.FullName;

            if (model.LanguageCode is not null)
                user.LanguageCode = model.LanguageCode;

            if (model.CurrencyCode is not null)
                user.CurrencyCode = model.CurrencyCode;

            if (model.TimeZone is not null)
                user.TimeZone = model.TimeZone;

            user.IsDarkMode = model.IsDarkMode;
            user.PushNotificationsEnabled = model.PushNotificationsEnabled;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new BadRequestException($"Failed to update profile: {FormatIdentityErrors(result)}");

            return true;
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordDto model)
        {
            var user = await GetAuthenticatedUserAsync();

            var isSamePassword = await _userManager.CheckPasswordAsync(user, model.NewPassword);
            if (isSamePassword)
                throw new BadRequestException("New password must be different from the current password.");

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
                throw new BadRequestException($"Failed to change password: {FormatIdentityErrors(result)}");

            await _tokenStore.RevokeRefreshToken(user.Id.ToString());

            await _userManager.UpdateSecurityStampAsync(user);

            return true;
        }

        private async Task<ApplicationUser> GetAuthenticatedUserAsync()
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("User is not authenticated.");

            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new NotFoundException("User not found.");

            return user;
        }

        private async Task<AuthResponseDTO> IssueAndStoreTokensAsync(ApplicationUser user)
        {
            var tokenResponse = await _jwtService.GenerateJWT(user);
            await _tokenStore.StoreRefreshToken(user.Id.ToString(), tokenResponse.RefreshToken);
            return tokenResponse;
        }

  
        private static string FormatIdentityErrors(IdentityResult result)
            => string.Join(", ", result.Errors.Select(e => e.Description));
    }
}