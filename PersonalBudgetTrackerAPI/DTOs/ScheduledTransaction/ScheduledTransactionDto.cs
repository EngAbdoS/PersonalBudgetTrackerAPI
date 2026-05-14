namespace PersonalBudgetTrackerAPI.DTOs.Entities.ScheduledTransactionDTOs
{
    public class ScheduledTransactionDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? TransactionDetails { get; set; }
        public decimal Amount { get; set; }
        public bool IsIncome { get; set; }
        public bool IsFlexibleAmount { get; set; }
        public string PaymentType { get; set; } = string.Empty;
        public Guid PaymentGatewayId { get; set; }
        public string PaymentGatewayName { get; set; } = string.Empty;
        public Guid TransactionPartnerId { get; set; }
        public string TransactionPartnerName { get; set; } = string.Empty;
        public Guid? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public decimal? FeeAmount { get; set; }
        public Guid? ReasonId { get; set; }
        public string? ReasonDetails { get; set; }
        public string PeriodType { get; set; } = string.Empty;
        public string? DayOfWeek { get; set; }
        public int? DayOfMonth { get; set; }
        public DateTime NextDueDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}