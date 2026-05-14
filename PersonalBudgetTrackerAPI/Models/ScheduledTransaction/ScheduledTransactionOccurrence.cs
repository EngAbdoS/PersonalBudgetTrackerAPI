namespace PersonalBudgetTrackerAPI.Models.ScheduledPayments
{
    public class ScheduledTransactionOccurrence
    {
        public Guid Id { get; set; }
        public Guid ScheduledTransactionId { get; set; }
        public ScheduledTransaction ScheduledTransaction { get; set; } = null!;
        public DateTime DueDate { get; set; }
        public OccurrenceStatus Status { get; set; }
    }
}
