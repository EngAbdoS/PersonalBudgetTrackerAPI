using PersonalBudgetTrackerAPI.Models.Entities;

namespace PersonalBudgetTrackerAPI.DTOs.Entities.PaymentGatewayDtos
{
    public class CreatePaymentGatewayDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public PaymentGatewayType PaymentGatewayType { get; set; }
        public decimal InitialBalance { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
