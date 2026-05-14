namespace PersonalBudgetTrackerAPI.DTOs.StatisticsDTOs
{
    public class CategorySpendingDto
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public decimal PercentageOfTotalExpenses { get; set; }
    }
}
