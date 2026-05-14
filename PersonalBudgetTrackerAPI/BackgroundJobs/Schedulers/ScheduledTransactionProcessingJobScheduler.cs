using Hangfire;
using PersonalBudgetTrackerAPI.BackgroundJobs.Interfaces;

namespace PersonalBudgetTrackerAPI.BackgroundJobs.Schedulers
{
    public class ScheduledTransactionProcessingJobScheduler
    {
        public static void Register(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var recurringJobManager =
                scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

            // Run every day at 08:00 UTC
            recurringJobManager.AddOrUpdate<IScheduledTransactionProcessingJob>(
                "scheduled-transaction-processing",
                job => job.RunAsync(),
                "0 8 * * *");
        }
    }
}
