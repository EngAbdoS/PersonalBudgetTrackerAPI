namespace PersonalBudgetTrackerAPI.BackgroundJobs.Interfaces
{
    public interface IScheduledTransactionProcessingJob
    {
        Task RunAsync();
    }
}
