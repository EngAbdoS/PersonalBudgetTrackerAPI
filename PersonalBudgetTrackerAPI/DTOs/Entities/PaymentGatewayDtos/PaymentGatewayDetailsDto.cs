namespace PersonalBudgetTrackerAPI.DTOs.Entities.PaymentGatewayDtos
{
    public class PaymentGatewayDetailsDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public int UsageCount { get; set; }

        public decimal CurrentBalance { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
    }
}
