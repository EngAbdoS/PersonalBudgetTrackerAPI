namespace PersonalBudgetTrackerAPI.DTOs.Entities.TransactionDTOs
{
    public class TransactionSimpleDto
    {
        public Guid TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? TransactionDetails { get; set; }
        public DateTime Date { get; set; }

    }
}
