using PersonalBudgetTrackerAPI.DTOs.StatisticsDTOs;

namespace PersonalBudgetTrackerAPI.Services.Interfaces.Statistics
{
    public interface IStatisticsService
    {
        Task<IEnumerable<CategorySpendingDto>> GetTopSpendingCategoriesAsync(int count, DateOnly from, DateOnly? to = null);
        Task<IEnumerable<TransactionPartnerSpendingDto>> GetTopSpendingPartnersAsync(int count, DateOnly from, DateOnly? to = null);
        Task<ExpenseIncomeRatioDto> GetExpenseVsIncomeRatioAsync(DateOnly from, DateOnly? to = null);
        Task<decimal> GetAverageDailyExpenseAsync(DateOnly from, DateOnly? to = null);
        Task<PeriodChangeDto> GetPeriodOverPeriodChangeAsync(DateOnly currentPeriodStart, DateOnly currentPeriodEnd);
        Task<DashboardStatisticsDto> GetDashboardSummaryAsync(DateOnly from, DateOnly? to = null);
    }
}
