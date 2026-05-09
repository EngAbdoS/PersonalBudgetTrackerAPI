namespace PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.DailySnapshot
{
    public class UserDailySnapshotDto
    {
        public Guid UserId { get; set; }
        public DateOnly Date { get; set; }
        public DailySnapshot Snapshot { get; set; } = null!;
    }
    public class DailySnapshot
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }

        public Dictionary<string, GatewaySnapshot> PaymentGateways { get; set; } = [];

        public Dictionary<string, decimal> SpendingCategories { get; set; } = [];

        public Dictionary<string, decimal> SpendingPartners { get; set; } = [];

        public Dictionary<string, decimal> IncomeFromPartners { get; set; } = [];
    }

    public class GatewaySnapshot
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public Dictionary<string, decimal> CategoriesSpendIn { get; set; } = [];
        public Dictionary<string, decimal> PartnersSpendWith { get; set; } = [];
    }
}
