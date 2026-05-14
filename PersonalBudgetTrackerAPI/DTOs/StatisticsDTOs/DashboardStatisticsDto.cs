using System.Collections.Generic;

namespace PersonalBudgetTrackerAPI.DTOs.StatisticsDTOs
{
    public class DashboardStatisticsDto
    {
        public ExpenseIncomeRatioDto ExpenseIncomeRatio { get; set; } = new();
        public decimal AverageDailyExpense { get; set; }
        public IEnumerable<CategorySpendingDto> TopSpendingCategories { get; set; } = new List<CategorySpendingDto>();
        public IEnumerable<TransactionPartnerSpendingDto> TopSpendingPartners { get; set; } = new List<TransactionPartnerSpendingDto>();
    }
}
