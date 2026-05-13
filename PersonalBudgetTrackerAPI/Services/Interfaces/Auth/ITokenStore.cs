namespace PersonalBudgetTrackerAPI.Services.Interfaces.Auth
{
    public interface ITokenStore
    {
        Task StoreRefreshToken(string userId, string token);
        Task<bool> ValidateRefreshToken(string userId, string token);
        Task RevokeRefreshToken(string userId);
    }
}
