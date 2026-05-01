namespace PersonalBudgetTrackerAPI.DTOs.Entities.TransactionDTOs
{
    public class TransactionRequirementsDto
    {
        public bool HasPaymentGateways { get; set; }
        public bool HasCategories { get; set; }
        public bool HasTransactionPartners { get; set; }

        public List<string> MissingResources { get; set; } = new();

        public object IncomeRequirements { get; set; } = new();
        public object ExpenseRequirements { get; set; } = new();
    }
}
