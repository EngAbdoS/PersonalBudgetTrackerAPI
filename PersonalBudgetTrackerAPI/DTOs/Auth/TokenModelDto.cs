namespace PersonalBudgetTrackerAPI.DTOs.Auth
{
    public class TokenModelDto
    {
        public string? Token { get; set; } = string.Empty;
        public string? RefreshToken { get; set; } = string.Empty;
    }
}
