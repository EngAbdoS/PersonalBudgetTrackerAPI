using Newtonsoft.Json;
using PersonalBudgetTrackerAPI.DTOs.Entities.TransactionDTOs;
using PersonalBudgetTrackerAPI.Services.Interfaces.FinanialPrefrances;
using StackExchange.Redis;

namespace PersonalBudgetTrackerAPI.Services.Implementations.FinanialPrefrances
{
    public class PendingTransactionCacheService : IPendingTransactionCacheService
    {
        private readonly IDatabase _db;
        private const int TtlMinutes = 10;

        private static string Key(Guid id) => $"pending-transaction:{id}";

        public PendingTransactionCacheService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task<Guid> CacheAsync(CreateTransactionDto dto)
        {
            var id = Guid.NewGuid();
            var json = JsonConvert.SerializeObject(dto);

            await _db.StringSetAsync(
                Key(id),
                json,
                TimeSpan.FromMinutes(TtlMinutes));

            return id;
        }

        public async Task<CreateTransactionDto?> GetAsync(Guid cachedId)
        {
            var redisValue = await _db.StringGetAsync(Key(cachedId));

            if (!redisValue.HasValue) return null;

            var json = redisValue.ToString();

            return JsonConvert.DeserializeObject<CreateTransactionDto>(json);
        }

        public async Task DeleteAsync(Guid cachedId)
        {
            await _db.KeyDeleteAsync(Key(cachedId));
        }
    }
}