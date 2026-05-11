using PersonalBudgetTrackerAPI.BackgroundJobs.Interfaces;
using PersonalBudgetTrackerAPI.Services.Interfaces;

namespace PersonalBudgetTrackerAPI.BackgroundJobs.Implementations
{
    public class SnapshotPromotionJob : ISnapshotPromotionJob
    {

        private readonly IDaySnapshotService _redisSnapshot;
        private readonly IDailySnapshotMongoService _mongoSnapshot;
        private readonly ILogger<SnapshotPromotionJob> _logger;

        public SnapshotPromotionJob(
            IDaySnapshotService redisSnapshot,
            IDailySnapshotMongoService mongoSnapshot,
            ILogger<SnapshotPromotionJob> logger)
        {
            _redisSnapshot = redisSnapshot;
            _mongoSnapshot = mongoSnapshot;
            _logger = logger;
        }


        public async Task RunAsync()
        {
            _logger.LogInformation("Snapshot promotion job started at {Time}", DateTime.UtcNow);

            var promoted = 0;
            var failed = 0;

            await foreach (var record in _redisSnapshot.GetYesterdaySnapshotsForAllUsersAsync())
            {
                var success = await _mongoSnapshot.UpsertSnapshotAsync(record);

                if (success)
                {
                    await _redisSnapshot.DeleteSnapshotAsync(record.UserId.ToString(), record.Date);
                    promoted++;
                    _logger.LogInformation("Promoted snapshot for user {UserId} date {Date}",
                        record.UserId, record.Date);
                }
                else
                {
                    failed++;
                    _logger.LogWarning("Failed to promote snapshot for user {UserId} date {Date} — keeping in Redis for retry",
                        record.UserId, record.Date);
                }
            }

            _logger.LogInformation(
                "Snapshot promotion job completed. Promoted: {Promoted}, Failed: {Failed}",
                promoted, failed);
        }
    }
}
