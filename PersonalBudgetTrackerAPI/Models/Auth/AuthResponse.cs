namespace PersonalBudgetTrackerAPI.Models.Auth
{
    public class AuthResponse
    {
        public string FullName { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;

        public DateTime Expiration { get; set; }

        public string Message { get; set; } = string.Empty;
    }

}
