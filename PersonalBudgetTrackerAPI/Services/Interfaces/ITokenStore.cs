namespace PersonalBudgetTrackerAPI.Services.Interfaces
{
    public interface ITokenStore
    {
        Task StoreRefreshToken(string userId, string token);
        Task<bool> ValidateRefreshToken(string userId, string token);
        Task RevokeRefreshToken(string userId);
    }
}
