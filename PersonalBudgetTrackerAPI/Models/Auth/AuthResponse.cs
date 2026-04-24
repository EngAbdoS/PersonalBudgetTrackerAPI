namespace PersonalBudgetTrackerAPI.Models.Auth
{
    public class AuthResponse
    {
        public string Token { get; set; } = null!;
        public string Message { get; set; } = string.Empty;
    }

}
