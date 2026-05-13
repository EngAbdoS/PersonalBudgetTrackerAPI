namespace PersonalBudgetTrackerAPI.DTOs.Entities.TransactionDTOs
{
    public class ExpenseTransactionValidationDto
    {
        public decimal Amount { get; set; }
        public Guid PaymentGatewayId { get; set; }
        public Guid CategoryId { get; set; }
        public Guid TransactionPartnerId { get; set; }
    }
}
