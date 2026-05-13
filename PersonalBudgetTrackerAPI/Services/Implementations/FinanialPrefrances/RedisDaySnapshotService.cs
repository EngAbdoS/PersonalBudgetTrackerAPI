using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.DailySnapshot;
using PersonalBudgetTrackerAPI.Services.Interfaces.FinanialPrefrances;
using StackExchange.Redis;

namespace PersonalBudgetTrackerAPI.Services.Implementations.FinanialPrefrances
{
    public class RedisDaySnapshotService : IDaySnapshotService
    {
        private readonly IDatabase _db;
        private readonly IServer _server;
        private readonly int _ttlHours;

        private static DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);
        private static DateOnly Yesterday => DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));

        public RedisDaySnapshotService(IConnectionMultiplexer redis, IConfiguration configuration)
        {
            _db = redis.GetDatabase();
            _server = redis.GetServer(redis.GetEndPoints().First());
            _ttlHours = configuration.GetValue<int>("DailySnapshot:TtlHours");

        }


        private static string SnapshotKey(string userId, DateOnly date) =>
            $"finance:{userId}:{date:yyyy-MM-dd}";


        private static string GatewayIncome(Guid gatewayId) => $"gateway:{gatewayId}:totalIncome";
        private static string GatewayExpense(Guid gatewayId) => $"gateway:{gatewayId}:totalExpense";
        private static string GatewayCategory(Guid gatewayId, Guid categoryId) => $"gateway:{gatewayId}:cat:{categoryId}";
        private static string GatewayPartner(Guid gatewayId, Guid partnerId) => $"gateway:{gatewayId}:partner:{partnerId}";
        private static string SpendingCategory(Guid categoryId) => $"cat:{categoryId}";
        private static string SpendingPartner(Guid partnerId) => $"partner:expense:{partnerId}";
        private static string IncomePartner(Guid partnerId) => $"partner:income:{partnerId}";


        public async Task ApplyExpenseAsync(
            string userId,
            Guid gatewayId,
            Guid categoryId,
            Guid partnerId,
            decimal amount,
            DateTime transactionDate)
        {
            var key = SnapshotKey(userId, DateOnly.FromDateTime(transactionDate));
            var batch = _db.CreateBatch();

            var tasks = new Task[]
            {
            batch.HashIncrementAsync(key, "totalTransactions", 1),
            batch.HashIncrementAsync(key, "totalExpense",                          (double)amount),
            batch.HashIncrementAsync(key, GatewayExpense(gatewayId),               (double)amount),
            batch.HashIncrementAsync(key, GatewayCategory(gatewayId, categoryId),  (double)amount),
            batch.HashIncrementAsync(key, GatewayPartner(gatewayId, partnerId),    (double)amount),
            batch.HashIncrementAsync(key, SpendingCategory(categoryId),            (double)amount),
            batch.HashIncrementAsync(key, SpendingPartner(partnerId),              (double)amount),
            batch.KeyExpireAsync(key, TimeSpan.FromHours(_ttlHours))
            };

            batch.Execute();
            await Task.WhenAll(tasks);
        }


        public async Task ApplyIncomeAsync(
            string userId,
            Guid gatewayId,
            Guid partnerId,
            decimal amount,
            DateTime transactionDate)
        {
            var key = SnapshotKey(userId, DateOnly.FromDateTime(transactionDate));
            var batch = _db.CreateBatch();

            var tasks = new Task[]
            {        
            batch.HashIncrementAsync(key, "totalTransactions", 1),
            batch.HashIncrementAsync(key, "totalIncome", (double)amount),
            batch.HashIncrementAsync(key, GatewayIncome(gatewayId), (double)amount),
            batch.HashIncrementAsync(key, IncomePartner(partnerId), (double)amount),
            batch.KeyExpireAsync(key, TimeSpan.FromHours(_ttlHours))
            };

            batch.Execute();
            await Task.WhenAll(tasks);
        }


        public async Task<DailySnapshot?> GetTodaySnapshotAsync(string userId)
        {
            var fields = await _db.HashGetAllAsync(SnapshotKey(userId, Today));
            return fields.Length == 0 ? null : Deserialize(fields);
        }

        public async IAsyncEnumerable<UserDailySnapshotDto> GetYesterdaySnapshotsForAllUsersAsync()
        {
            await foreach (var record in ScanSnapshotsAsync(Yesterday))
                yield return record;
        }

        public async IAsyncEnumerable<UserDailySnapshotDto> GetCurrentDaySnapshotsForAllUsersAsync()
        {
            await foreach (var record in ScanSnapshotsAsync(Today))
                yield return record;
        }
      
        public async Task DeleteSnapshotAsync(string  userId, DateOnly date)
        {
            await _db.KeyDeleteAsync(SnapshotKey(userId, date));
        }
        private async IAsyncEnumerable<UserDailySnapshotDto> ScanSnapshotsAsync(DateOnly date)
        {
            var pattern = $"finance:*:{date:yyyy-MM-dd}";

            await foreach (var key in _server.KeysAsync(pattern: pattern, pageSize: 100))
            {
                var parts = key.ToString().Split(':');
                var userId = Guid.Parse(parts[1]);
                var fields = await _db.HashGetAllAsync(key);

                if (fields.Length == 0) continue;

                yield return new UserDailySnapshotDto
                {
                    UserId = userId,
                    Date = date,
                    Snapshot = Deserialize(fields)
                };
            }
        }
        private static DailySnapshot Deserialize(HashEntry[] fields)
        {
            var snapshot = new DailySnapshot();

            foreach (var field in fields)
            {
                var name = field.Name.ToString();
                var value = (decimal)(double)field.Value;

                switch (name.Split(':'))
                {
                    case ["totalTransactions"]:
                        snapshot.TotalTransactions = (int)value; break;

                    case ["totalIncome"]:
                        snapshot.TotalIncome = value; break;

                    case ["totalExpense"]:
                        snapshot.TotalExpense = value; break;

                    case ["cat", var catId]:
                        snapshot.SpendingCategories[catId] = value; break;

                    case ["partner", "expense", var partnerId]:
                        snapshot.SpendingPartners[partnerId] = value; break;

                    case ["partner", "income", var partnerId]:
                        snapshot.IncomeFromPartners[partnerId] = value; break;

                    case ["gateway", var gwId, "totalIncome"]:
                        GetOrAddGateway(snapshot, gwId).TotalIncome = value; break;

                    case ["gateway", var gwId, "totalExpense"]:
                        GetOrAddGateway(snapshot, gwId).TotalExpense = value; break;

                    case ["gateway", var gwId, "cat", var catId]:
                        GetOrAddGateway(snapshot, gwId).CategoriesSpendIn[catId] = value; break;

                    case ["gateway", var gwId, "partner", var partnerId]:
                        GetOrAddGateway(snapshot, gwId).PartnersSpendWith[partnerId] = value; break;
                }
            }

            return snapshot;
        }

        private static GatewaySnapshot GetOrAddGateway(DailySnapshot snapshot, string gwId)
        {
            if (!snapshot.PaymentGateways.TryGetValue(gwId, out var gateway))
            {
                gateway = new GatewaySnapshot();
                snapshot.PaymentGateways[gwId] = gateway;
            }
            return gateway;
        }
    }
}
