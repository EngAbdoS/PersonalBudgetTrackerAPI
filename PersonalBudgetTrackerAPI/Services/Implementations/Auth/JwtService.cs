using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using NuGet.Packaging;
using PersonalBudgetTrackerAPI.DTOs.Auth;
using PersonalBudgetTrackerAPI.Identity;
using PersonalBudgetTrackerAPI.Services.Interfaces.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PersonalBudgetTrackerAPI.Services.Implementations.Auth
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        public JwtService(IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<AuthResponseDTO> GenerateJWT(ApplicationUser user)
        {

            DateTime expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireTimeMinuts"]));
            Claim[] claims = 
            [
                new Claim (JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),ClaimValueTypes.Integer64),
                new Claim(ClaimTypes.NameIdentifier, user.Email??""),
                new Claim(ClaimTypes.Email, user.Email??""),
                new Claim(ClaimTypes.Name, user.FullName??""),
            ];

            var userRoles = await _userManager.GetRolesAsync(user);

            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));  

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]!));

            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken tokenGenerator = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            string token = tokenHandler.WriteToken(tokenGenerator);

            return new AuthResponseDTO
            {
                Token = token,
                Expiration = expiration,
                Email = user.Email ?? "",
                FullName = user.FullName??"",
                RefreshToken = GenerateRefreshToken(),
                RefreshTokenExpirationDate = DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["RefreshToken:ExpireTimeMinuts"]))

            };
        }


        public ClaimsPrincipal? GetPrincipalFromJWTToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
               Encoding.UTF8.GetBytes(_configuration["Jwt:key"]!)
                )
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal claims = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return claims;

        }


        private string GenerateRefreshToken()
        {
           return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }
    }
}
