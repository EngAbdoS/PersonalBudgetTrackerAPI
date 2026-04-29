using PersonalBudgetTrackerAPI.Models.Entities;

namespace PersonalBudgetTrackerAPI.DTOs.Entities.PaymentGatewayDtos
{
    public class PaymentGatewayDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public PaymentGatewayType PaymentGatewayType { get; set; }

    }
}
