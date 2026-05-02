namespace PersonalBudgetTrackerAPI.DTOs.Entities.PaymentGatewayDtos
{
    public class PaymentGatewayDetailsDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public int IncomeUsageCount { get; set; }
        public int ExpenseUsageCount { get; set; }
        public int UsageCount => IncomeUsageCount + ExpenseUsageCount;

        public decimal CurrentBalance { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
