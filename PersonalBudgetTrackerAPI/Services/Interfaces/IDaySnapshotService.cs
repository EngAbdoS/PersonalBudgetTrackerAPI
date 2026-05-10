using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.DailySnapshot;

namespace PersonalBudgetTrackerAPI.Services.Interfaces
{
    public interface IDaySnapshotService
    {
        Task ApplyExpenseAsync(string userId, Guid gatewayId, Guid categoryId, Guid partnerId, decimal amount, DateTime transactionDate);
        Task ApplyIncomeAsync(string userId, Guid gatewayId, Guid partnerId, decimal amount, DateTime transactionDate);
        Task<DailySnapshot?> GetTodaySnapshotAsync(string userId);
        IAsyncEnumerable<UserDailySnapshotDto> GetYesterdaySnapshotsForAllUsersAsync();
        IAsyncEnumerable<UserDailySnapshotDto> GetCurrentDaySnapshotsForAllUsersAsync();
        Task DeleteSnapshotAsync(string userId, DateOnly date);

    }
}
