using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.DailySnapshot;
using PersonalBudgetTrackerAPI.MongoDB.Documents;

namespace PersonalBudgetTrackerAPI.Services.Interfaces
{
    public interface IDailySnapshotMongoService
    {
        Task<bool> UpsertSnapshotAsync(UserDailySnapshotDto dto);
        Task<IEnumerable<DailySnapshotDocument>> GetSnapshotsRangeAsync(Guid userId, DateOnly from, DateOnly to);

    }
}
