namespace PersonalBudgetTrackerAPI.DTOs.Auth
{
    public class AuthResponseDTO
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;

        public DateTime Expiration { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpirationDate { get; set; }
        public string Message { get; set; } = string.Empty;
    }

}
