using Hangfire;
using PersonalBudgetTrackerAPI.BackgroundJobs.Interfaces;

namespace PersonalBudgetTrackerAPI.BackgroundJobs.Schedulers
{
    public class SnapshotPromotionJobScheduler
    {
        public static void Register(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var recurringJobManager =
                scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

            recurringJobManager.AddOrUpdate<ISnapshotPromotionJob>(
                "snapshot-promotion-main",
                job => job.RunAsync(),
                "5 0 * * *"); // 00:05 UTC daily
                          
            
            // Double check job — runs at 01:00 every day
            recurringJobManager.AddOrUpdate<ISnapshotPromotionJob>(
                "snapshot-promotion-doublecheck",
                job => job.RunAsync(),
                "0 1 * * *");
        }
    }
}
