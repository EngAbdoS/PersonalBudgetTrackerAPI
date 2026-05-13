using Hangfire;
using PersonalBudgetTrackerAPI.BackgroundJobs.Interfaces;

namespace PersonalBudgetTrackerAPI.BackgroundJobs.Schedulers
{
    public class RuleActivationJobScheduler
    {
        public static void Register(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var recurringJobManager = scope.ServiceProvider
                .GetRequiredService<IRecurringJobManager>();

            recurringJobManager.AddOrUpdate<IRuleActivationJob>(
                "rule-activation",
                job => job.RunAsync(),
                "5 0 * * *");
        }
    }

}
