using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.DailySnapshot;
using PersonalBudgetTrackerAPI.MongoDB.Documents;
using PersonalBudgetTrackerAPI.MongoDB.Settings;
using PersonalBudgetTrackerAPI.Services.Interfaces;

namespace PersonalBudgetTrackerAPI.Services.Implementations
{
    public class DailySnapshotMongoService : IDailySnapshotMongoService
    {
        private readonly IMongoCollection<MonthlySnapshotDocument> _collection;

        public DailySnapshotMongoService(
            IMongoClient client,
            IOptions<MongoDbSettings> settings)
        {
            var db = client.GetDatabase(settings.Value.DatabaseName);

            _collection = db.GetCollection<MonthlySnapshotDocument>(
                settings.Value.SnapshotsCollection);

            EnsureIndexes();
        }

        public async Task<bool> UpsertSnapshotAsync(UserDailySnapshotDto dto)
        {
            try
            {
                var id = BuildMonthlyId(dto.UserId, dto.Date);
                var dayKey = dto.Date.ToString("yyyy-MM-dd");

                var filter = Builders<MonthlySnapshotDocument>.Filter
                    .Eq(d => d.Id, id);

                var update = Builders<MonthlySnapshotDocument>.Update
                    .Set(d => d.Days[dayKey], MapToDocument(dto.Snapshot))
                    .Set(d => d.UpdatedAt, DateTime.UtcNow)
                    .SetOnInsert(d => d.Id, id)
                    .SetOnInsert(d => d.UserId, dto.UserId)
                    .SetOnInsert(d => d.Month, $"{dto.Date:yyyy-MM}");

                var options = new UpdateOptions { IsUpsert = true };

                await _collection.UpdateOneAsync(filter, update, options);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MongoDB upsert failed: {ex.Message}");
                return false;
            }
        }


        public async Task<IEnumerable<DailySnapshotDocument>> GetSnapshotsRangeAsync(
            Guid userId,
            DateOnly from,
            DateOnly to)
        {
            var monthIds = EachMonth(from, to)
                .Select(m => BuildMonthlyId(userId, m))
                .ToList();

            var filter = Builders<MonthlySnapshotDocument>.Filter
                .In(d => d.Id, monthIds);

            var monthlyDocs = await _collection
                .Find(filter)
                .ToListAsync();

            return monthlyDocs
                .SelectMany(m => m.Days)
                .Where(kvp => DateOnly.Parse(kvp.Key) >= from &&
                              DateOnly.Parse(kvp.Key) <= to)
                .OrderBy(kvp => kvp.Key)
                .Select(kvp => kvp.Value)
                .ToList();
        }


        private void EnsureIndexes()
        {
            var indexModel = new CreateIndexModel<MonthlySnapshotDocument>(
                Builders<MonthlySnapshotDocument>.IndexKeys
                    .Ascending(d => d.UserId)
                    .Ascending(d => d.Month),
                new CreateIndexOptions { Unique = false }
            );

            _collection.Indexes.CreateOne(indexModel);
        }


        private static string BuildMonthlyId(Guid userId, DateOnly date) =>
            $"{userId}:{date:yyyy-MM}";

        private static IEnumerable<DateOnly> EachMonth(DateOnly from, DateOnly to)
        {
            var current = new DateOnly(from.Year, from.Month, 1);
            while (current <= to)
            {
                yield return current;
                current = current.AddMonths(1);
            }
        }


        private static DailySnapshotDocument MapToDocument(DailySnapshot snapshot) => new()
        {
            TotalTransactions = snapshot.TotalTransactions,
            TotalIncome = snapshot.TotalIncome,
            TotalExpense = snapshot.TotalExpense,
            SpendingCategories = snapshot.SpendingCategories,
            SpendingPartners = snapshot.SpendingPartners,
            IncomeFromPartners = snapshot.IncomeFromPartners,
            PaymentGateways = snapshot.PaymentGateways
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => new GatewaySnapshotDocument
                    {
                        TotalIncome = kvp.Value.TotalIncome,
                        TotalExpense = kvp.Value.TotalExpense,
                        CategoriesSpendIn = kvp.Value.CategoriesSpendIn,
                        PartnersSpendWith = kvp.Value.PartnersSpendWith
                    })
        };
    }
}
