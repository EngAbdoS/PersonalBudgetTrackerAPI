using PersonalBudgetTrackerAPI.Models.Entities;

namespace PersonalBudgetTrackerAPI.DTOs.Entities.TransactionDTOs
{
    public class TransactionDto
    {
        public Guid TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? TransactionDetails { get; set; }
        public DateTime Date { get; set; }

        public string Type { get; set; } = string.Empty;

        public PaymentType PaymentType { get; set; }

        public string PaymentGateway { get; set; } = string.Empty;

        public Guid PaymentGatewayId { get; set; }
        public string TransactionPartner { get; set; } = string.Empty;
        public Guid TransactionPartnerId { get; set; }

        public string? Reason { get; set; }
        public Guid ReasonId { get; set; }
        public string? Category { get; set; }
        public Guid CategoryId { get; set; }
    }
}
