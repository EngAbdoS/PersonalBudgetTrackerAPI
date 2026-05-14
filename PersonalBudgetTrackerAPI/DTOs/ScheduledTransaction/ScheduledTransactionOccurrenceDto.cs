namespace PersonalBudgetTrackerAPI.DTOs.ScheduledTransaction
{
    public class ScheduledTransactionOccurrenceDto
    {
        public Guid Id { get; set; }
        public Guid ScheduledTransactionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public bool IsFlexibleAmount { get; set; }
        public bool IsIncome { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentGatewayName { get; set; } = string.Empty;
        public string TransactionPartnerName { get; set; } = string.Empty;
        public string? CategoryName { get; set; }
        public string? ReasonDetails { get; set; }
    }
}
