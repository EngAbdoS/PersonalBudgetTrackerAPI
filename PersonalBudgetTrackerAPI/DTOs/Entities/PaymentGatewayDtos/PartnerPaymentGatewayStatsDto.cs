namespace PersonalBudgetTrackerAPI.DTOs.Entities.PaymentGatewayDtos
{
    public class PartnerPaymentGatewayStatsDto
    {
        public Guid PaymentGatewayId { get; set; }
        public string Title { get; set; } = string.Empty;

        public int UsageCount { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
    }
}
