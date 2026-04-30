namespace PersonalBudgetTrackerAPI.DTOs.Entities.TransactionPartnerDTOs
{
    public class CreateTransactionPartnerDto
    {
        public string Name { get; set; } = string.Empty;
        public string Info { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Contact { get; set; } = string.Empty;
    }
}
