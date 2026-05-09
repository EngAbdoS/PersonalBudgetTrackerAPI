using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.DailySnapshot;
using PersonalBudgetTrackerAPI.Services.Interfaces;

namespace PersonalBudgetTrackerAPI.Services.Implementations
{
    public class RedisDaySnapshotService : IDaySnapshotService
    {
        public Task ApplyExpenseAsync(string userId, Guid gatewayId, Guid categoryId, Guid partnerId, decimal amount, DateTime transactionDate)
        {
            throw new NotImplementedException();
        }

        public Task ApplyIncomeAsync(string userId, Guid gatewayId, Guid partnerId, decimal amount, DateTime transactionDate)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, DailySnapshot>> GetCurrentDaySnapshotsForAllUsersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<DailySnapshot?> GetTodaySnapshotAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, DailySnapshot>> GetYesterdaySnapshotsForAllUsersAsync()
        {
            throw new NotImplementedException();
        }
    }
}
