using PersonalBudgetTrackerAPI.DatabaseContext;
using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.DailySnapshot;
using PersonalBudgetTrackerAPI.DTOs.StatisticsDTOs;
using PersonalBudgetTrackerAPI.Services.Interfaces.FinanialPrefrances;
using PersonalBudgetTrackerAPI.Services.Interfaces.Statistics;
using Microsoft.EntityFrameworkCore;

public class StatisticsService : IStatisticsService
{
    private readonly IFinancialAggregatorService _aggregatorService;
    private readonly ApplicationDbContext _dbContext;

    public StatisticsService(
        IFinancialAggregatorService aggregatorService,
        ApplicationDbContext dbContext)
    {
        _aggregatorService = aggregatorService;
        _dbContext = dbContext;
    }

    // ── Dashboard Summary ─────────────────────────────────────
    // Fetches snapshots ONCE and passes to all sub-calculations

    public async Task<DashboardStatisticsDto> GetDashboardSummaryAsync(
        DateOnly from,
        DateOnly? to = null)
    {
        // single data fetch — shared across all calculations
        var dailySnapshots = await _aggregatorService.GetDailySnapshotsAsync(from, to);
        var snapshots = dailySnapshots.Select(s => s.Snapshot).ToList();

        if (!snapshots.Any())
            return new DashboardStatisticsDto();

        var ratio = ComputeExpenseIncomeRatio(snapshots);
        var averageExpense = ComputeAverageDailyExpense(snapshots, from, to);
        var topCategories = await ComputeTopCategoriesAsync(snapshots, 5);
        var topPartners = await ComputeTopPartnersAsync(snapshots, 5);

        return new DashboardStatisticsDto
        {
            ExpenseIncomeRatio = ratio,
            AverageDailyExpense = averageExpense,
            TopSpendingCategories = topCategories,
            TopSpendingPartners = topPartners
        };
    }

    // ── Top Spending Categories ───────────────────────────────

    public async Task<IEnumerable<CategorySpendingDto>> GetTopSpendingCategoriesAsync(
        int count,
        DateOnly from,
        DateOnly? to = null)
    {
        count = Math.Max(1, count); // guard against 0 or negative

        var dailySnapshots = await _aggregatorService.GetDailySnapshotsAsync(from, to);
        var snapshots = dailySnapshots.Select(s => s.Snapshot).ToList();

        return await ComputeTopCategoriesAsync(snapshots, count);
    }

    // ── Top Spending Partners ─────────────────────────────────

    public async Task<IEnumerable<TransactionPartnerSpendingDto>> GetTopSpendingPartnersAsync(
        int count,
        DateOnly from,
        DateOnly? to = null)
    {
        count = Math.Max(1, count);

        var dailySnapshots = await _aggregatorService.GetDailySnapshotsAsync(from, to);
        var snapshots = dailySnapshots.Select(s => s.Snapshot).ToList();

        return await ComputeTopPartnersAsync(snapshots, count);
    }

    // ── Expense vs Income Ratio ───────────────────────────────

    public async Task<ExpenseIncomeRatioDto> GetExpenseVsIncomeRatioAsync(
        DateOnly from,
        DateOnly? to = null)
    {
        var dailySnapshots = await _aggregatorService.GetDailySnapshotsAsync(from, to);
        var snapshots = dailySnapshots.Select(s => s.Snapshot).ToList();

        return ComputeExpenseIncomeRatio(snapshots);
    }

    // ── Average Daily Expense ─────────────────────────────────

    public async Task<decimal> GetAverageDailyExpenseAsync(
        DateOnly from,
        DateOnly? to = null)
    {
        var dailySnapshots = await _aggregatorService.GetDailySnapshotsAsync(from, to);
        var snapshots = dailySnapshots.Select(s => s.Snapshot).ToList();

        return ComputeAverageDailyExpense(snapshots, from, to);
    }

    // ── Period Over Period Change ─────────────────────────────
    // Fetches both periods in parallel — two data fetches not four

    public async Task<PeriodChangeDto> GetPeriodOverPeriodChangeAsync(
        DateOnly currentPeriodStart,
        DateOnly currentPeriodEnd)
    {
        var daysInPeriod = (currentPeriodEnd.DayNumber - currentPeriodStart.DayNumber) + 1;
        var prevPeriodStart = currentPeriodStart.AddDays(-daysInPeriod);
        var prevPeriodEnd = currentPeriodStart.AddDays(-1);

        // fetch both periods in parallel
        var currentTask = _aggregatorService.GetDailySnapshotsAsync(currentPeriodStart, currentPeriodEnd);
        var prevTask = _aggregatorService.GetDailySnapshotsAsync(prevPeriodStart, prevPeriodEnd);

        await Task.WhenAll(currentTask, prevTask);

        var currentSnapshots = currentTask.Result.Select(s => s.Snapshot).ToList();
        var prevSnapshots = prevTask.Result.Select(s => s.Snapshot).ToList();

        return new PeriodChangeDto
        {
            CurrentIncome = currentSnapshots.Sum(s => s.TotalIncome),
            PreviousIncome = prevSnapshots.Sum(s => s.TotalIncome),
            CurrentExpense = currentSnapshots.Sum(s => s.TotalExpense),
            PreviousExpense = prevSnapshots.Sum(s => s.TotalExpense)
        };
    }

    // ── Private Compute Helpers ───────────────────────────────
    // Accept already-fetched snapshots — no extra DB/Redis calls

    private static ExpenseIncomeRatioDto ComputeExpenseIncomeRatio(
        List<DailySnapshot> snapshots) => new()
        {
            TotalIncome = snapshots.Sum(s => s.TotalIncome),
            TotalExpense = snapshots.Sum(s => s.TotalExpense)
        };

    private static decimal ComputeAverageDailyExpense(
        List<DailySnapshot> snapshots,
        DateOnly from,
        DateOnly? to)
    {
        var totalExpense = snapshots.Sum(s => s.TotalExpense);
        var end = to ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var days = Math.Max(1, (end.DayNumber - from.DayNumber) + 1);

        return totalExpense / days;
    }

    private async Task<IEnumerable<CategorySpendingDto>> ComputeTopCategoriesAsync(
        List<DailySnapshot> snapshots,
        int count)
    {
        var totalExpense = snapshots.Sum(s => s.TotalExpense);

        if (totalExpense == 0) return [];

        // aggregate category totals across all days
        var categoryTotals = snapshots
            .SelectMany(s => s.SpendingCategories)
            .Where(kvp => Guid.TryParse(kvp.Key, out _))
            .GroupBy(kvp => Guid.Parse(kvp.Key))
            .ToDictionary(
                g => g.Key,
                g => g.Sum(kvp => kvp.Value));

        if (!categoryTotals.Any()) return [];

        var topIds = categoryTotals
            .OrderByDescending(kv => kv.Value)
            .Take(count)
            .Select(kv => kv.Key)
            .ToList();

        // single DB query for all names
        var names = await _dbContext.Category
            .Where(c => topIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.Title);

        return topIds.Select(id => new CategorySpendingDto
        {
            CategoryId = id,
            CategoryName = names.GetValueOrDefault(id, "Unknown"),
            TotalAmount = categoryTotals[id],
            PercentageOfTotalExpenses = categoryTotals[id] / totalExpense * 100
        });
    }

    private async Task<IEnumerable<TransactionPartnerSpendingDto>> ComputeTopPartnersAsync(
        List<DailySnapshot> snapshots,
        int count)
    {
        var totalExpense = snapshots.Sum(s => s.TotalExpense);

        if (totalExpense == 0) return [];

        var partnerTotals = snapshots
            .SelectMany(s => s.SpendingPartners)
            .Where(kvp => Guid.TryParse(kvp.Key, out _))
            .GroupBy(kvp => Guid.Parse(kvp.Key))
            .ToDictionary(
                g => g.Key,
                g => g.Sum(kvp => kvp.Value));

        if (!partnerTotals.Any()) return [];

        var topIds = partnerTotals
            .OrderByDescending(kv => kv.Value)
            .Take(count)
            .Select(kv => kv.Key)
            .ToList();

        var names = await _dbContext.TransactionPartner
            .Where(tp => topIds.Contains(tp.Id))
            .ToDictionaryAsync(tp => tp.Id, tp => tp.Name);

        return topIds.Select(id => new TransactionPartnerSpendingDto
        {
            PartnerId = id,
            PartnerName = names.GetValueOrDefault(id, "Unknown"),
            TotalAmount = partnerTotals[id],
            PercentageOfTotalExpenses = partnerTotals[id] / totalExpense * 100
        });
    }
}