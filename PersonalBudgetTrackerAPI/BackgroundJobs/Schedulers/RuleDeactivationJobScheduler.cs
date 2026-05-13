using Hangfire;
using PersonalBudgetTrackerAPI.BackgroundJobs.Interfaces;

namespace PersonalBudgetTrackerAPI.BackgroundJobs.Schedulers
{
    public class RuleDeactivationJobScheduler
    {
        public static void Register(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var recurringJobManager = scope.ServiceProvider
                .GetRequiredService<IRecurringJobManager>();

            recurringJobManager.AddOrUpdate<IRuleDeactivationJob>(
                "rule-deactivation",
                job => job.RunAsync(),
                "1 0 * * *");
        }
    }
}
