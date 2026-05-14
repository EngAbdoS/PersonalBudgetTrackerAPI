namespace PersonalBudgetTrackerAPI.DTOs.StatisticsDTOs
{
    public class ExpenseIncomeRatioDto
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal NetSavings => TotalIncome - TotalExpense;
        public decimal SavingsRatePercentage => TotalIncome > 0 ? (NetSavings / TotalIncome) * 100 : 0;
    }
}
