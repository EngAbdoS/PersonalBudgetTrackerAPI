using PersonalBudgetTrackerAPI.DTOs.Entities.PaymentGatewayDtos;

namespace PersonalBudgetTrackerAPI.DTOs.Entities.TransactionPartnerDTOs
{
    public class TransactionPartnerDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public int UsageCount { get; set; }

        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }

        public List<string> Categories { get; set; } = new();
        public List<string> Reasons { get; set; } = new();

        public List<PartnerPaymentGatewayStatsDto> PaymentGateways { get; set; } = new();
    }
}
