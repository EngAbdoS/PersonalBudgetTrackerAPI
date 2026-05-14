namespace PersonalBudgetTrackerAPI.DTOs.StatisticsDTOs
{
    public class PeriodChangeDto
    {
        public decimal CurrentIncome { get; set; }
        public decimal PreviousIncome { get; set; }
        public decimal IncomeChangePercentage => PreviousIncome > 0 ? ((CurrentIncome - PreviousIncome) / PreviousIncome) * 100 : 0;

        public decimal CurrentExpense { get; set; }
        public decimal PreviousExpense { get; set; }
        public decimal ExpenseChangePercentage => PreviousExpense > 0 ? ((CurrentExpense - PreviousExpense) / PreviousExpense) * 100 : 0;
    }
}
