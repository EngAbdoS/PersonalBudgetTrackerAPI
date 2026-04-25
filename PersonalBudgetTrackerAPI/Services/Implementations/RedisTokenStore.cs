using PersonalBudgetTrackerAPI.Services.Interfaces;
using StackExchange.Redis;

namespace PersonalBudgetTrackerAPI.Services.Implementations
{
    public class RedisTokenStore : ITokenStore
    {
        private readonly IDatabase _db;
        private readonly IConfiguration _configuration;             

        public RedisTokenStore(IConnectionMultiplexer redis, IConfiguration configuration)
        {
            _db = redis.GetDatabase();
            _configuration = configuration;
        }

        public async Task StoreRefreshToken(string userId, string token)
        {
            await _db.StringSetAsync(
                $"refresh:{userId}",
                token,
                TimeSpan.FromMinutes(Convert.ToDouble(_configuration["RefreshToken:ExpireTimeMinuts"])));

        }

        public async Task<bool> ValidateRefreshToken(string userId, string token)
        {
            var storedToken = await _db.StringGetAsync($"refresh:{userId}");
            return storedToken == token;
        }

        public async Task RevokeRefreshToken(string userId)
        {
            await _db.KeyDeleteAsync($"refresh:{userId}");
        }

    }
}
