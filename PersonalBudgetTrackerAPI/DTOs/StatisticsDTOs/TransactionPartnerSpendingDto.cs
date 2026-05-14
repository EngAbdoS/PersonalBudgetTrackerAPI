namespace PersonalBudgetTrackerAPI.DTOs.StatisticsDTOs
{
    public class TransactionPartnerSpendingDto
    {
        public Guid PartnerId { get; set; }
        public string PartnerName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public decimal PercentageOfTotalExpenses { get; set; }
    }
}
