using Microsoft.EntityFrameworkCore;
using PersonalBudgetTrackerAPI.BackgroundJobs.Interfaces;
using PersonalBudgetTrackerAPI.DatabaseContext;
using PersonalBudgetTrackerAPI.Models.ScheduledTransaction;

namespace PersonalBudgetTrackerAPI.BackgroundJobs.Implementations
{
    public class ScheduledTransactionProcessingJob : IScheduledTransactionProcessingJob
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ScheduledTransactionProcessingJob> _logger;

        public ScheduledTransactionProcessingJob(
            ApplicationDbContext context,
            ILogger<ScheduledTransactionProcessingJob> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            var now = DateTime.UtcNow;
            _logger.LogInformation("ScheduledTransactionProcessingJob started at {Time}", now);

            var dueSchedules = await _context.ScheduledTransactions
                .IgnoreQueryFilters()
                .Where(st => st.IsActive && !st.IsDeleted && st.NextDueDate <= now)
                .ToListAsync();

            _logger.LogInformation("Found {Count} due scheduled transactions.", dueSchedules.Count);

            var created = 0;

            foreach (var st in dueSchedules)
            {
                try
                {
                    // Create a Pending occurrence for this due date
                    var occurrence = new ScheduledTransactionOccurrence
                    {
                        Id = Guid.NewGuid(),
                        ScheduledTransactionId = st.Id,
                        DueDate = st.NextDueDate,
                        Status = OccurrenceStatus.Pending
                    };

                    _context.ScheduledTransactionOccurrences.Add(occurrence);

                    // Advance NextDueDate based on PeriodType
                    st.NextDueDate = ComputeNextDueDate(st);

                    created++;
                    _logger.LogInformation(
                        "Created occurrence for ScheduledTransaction {Id}. Next due: {NextDue}",
                        st.Id, st.NextDueDate);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Failed to process ScheduledTransaction {Id}", st.Id);
                }
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "ScheduledTransactionProcessingJob completed. Occurrences created: {Count}", created);
        }


        private static DateTime ComputeNextDueDate(Models.ScheduledTransaction.ScheduledTransaction st)
        {
            var current = st.NextDueDate;

            return st.PeriodType switch
            {
                Models.FinancialPrefrances.PeriodType.Daily =>
                    current.AddDays(1),

                Models.FinancialPrefrances.PeriodType.Weekly =>
                    current.AddDays(7),

                Models.FinancialPrefrances.PeriodType.Monthly =>
                    AdvanceByOneMonth(current, st.DayOfMonth),

                _ => throw new InvalidOperationException(
                    $"Unsupported PeriodType '{st.PeriodType}' for scheduled transaction {st.Id}.")
            };
        }

        private static DateTime AdvanceByOneMonth(DateTime current, int? preferredDay)
        {
            var next = current.AddMonths(1);

            if (preferredDay.HasValue)
            {
                var daysInMonth = DateTime.DaysInMonth(next.Year, next.Month);
                var day = Math.Min(preferredDay.Value, daysInMonth);
                next = new DateTime(next.Year, next.Month, day,
                    current.Hour, current.Minute, current.Second, DateTimeKind.Utc);
            }

            return next;
        }
    }
}
