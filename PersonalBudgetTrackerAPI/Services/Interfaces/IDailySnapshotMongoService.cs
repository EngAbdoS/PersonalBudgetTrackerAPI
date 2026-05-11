using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.DailySnapshot;

namespace PersonalBudgetTrackerAPI.Services.Interfaces
{
    public interface IDailySnapshotMongoService
    {
        Task<bool> UpsertSnapshotAsync(UserDailySnapshotDto dto);
        Task<IEnumerable<UserDailySnapshotDto>> GetSnapshotsRangeAsync( DateOnly from, DateOnly to);

    }
}
