using PersonalBudgetTrackerAPI.DTOs.Entities.CategoryDTOs;
using PersonalBudgetTrackerAPI.DTOs.Entities.PaymentGatewayDtos;
using PersonalBudgetTrackerAPI.DTOs.Entities.TransactionPartnerDTOs;
using PersonalBudgetTrackerAPI.Models.Entities;

namespace PersonalBudgetTrackerAPI.DTOs.Entities.TransactionDTOs
{
    public class CreateTransactionDto
    {
        public decimal Amount { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? TransactionDetails { get; set; }
        public DateTime Date { get; set; }

        public PaymentType PaymentType { get; set; }

        public bool IsIncome { get; set; }

        public Guid? PaymentGatewayId { get; set; }
        public CreatePaymentGatewayDto? NewPaymentGateway { get; set; }

        public Guid? TransactionPartnerId { get; set; }
        public CreateTransactionPartnerDto? NewPartner { get; set; }

        // Income
        public Guid? ReasonId { get; set; }
        public string? NewReason { get; set; }

        // Expense
        public Guid? CategoryId { get; set; }
        public CreateCategoryDto? NewCategory { get; set; }
        public decimal? FeeAmount { get; set; }
    }
}
