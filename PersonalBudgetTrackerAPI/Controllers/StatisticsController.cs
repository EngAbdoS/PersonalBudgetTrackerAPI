using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalBudgetTrackerAPI.DTOs.StatisticsDTOs;
using PersonalBudgetTrackerAPI.Services.Interfaces.Statistics;

namespace PersonalBudgetTrackerAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        [HttpGet("dashboard-summary")]
        public async Task<ActionResult<DashboardStatisticsDto>> GetDashboardSummary([FromQuery] DateOnly from, [FromQuery] DateOnly? to)
        {
            var result = await _statisticsService.GetDashboardSummaryAsync(from, to);
            return Ok(result);
        }

        [HttpGet("top-categories")]
        public async Task<ActionResult<IEnumerable<CategorySpendingDto>>> GetTopCategories(
             [FromQuery] DateOnly from,
             [FromQuery] DateOnly? to,
             [FromQuery] int count = 5)
        {
            if (count < 1 || count > 50)
                return BadRequest("Count must be between 1 and 50");

            var result = await _statisticsService.GetTopSpendingCategoriesAsync(count, from, to);
            return Ok(result);
        }

        [HttpGet("top-partners")]
        public async Task<ActionResult<IEnumerable<TransactionPartnerSpendingDto>>> GetTopPartners(
            [FromQuery] DateOnly from,
            [FromQuery] DateOnly? to,
            [FromQuery] int count = 5)
        {
            if (count < 1 || count > 50)
                return BadRequest("Count must be between 1 and 50");

            var result = await _statisticsService.GetTopSpendingPartnersAsync(count, from, to);
            return Ok(result);
        }

        [HttpGet("expense-income-ratio")]
        public async Task<ActionResult<ExpenseIncomeRatioDto>> GetExpenseIncomeRatio([FromQuery] DateOnly from, [FromQuery] DateOnly? to)
        {
            var result = await _statisticsService.GetExpenseVsIncomeRatioAsync(from, to);
            return Ok(result);
        }

        [HttpGet("average-daily-expense")]
        public async Task<ActionResult<decimal>> GetAverageDailyExpense([FromQuery] DateOnly from, [FromQuery] DateOnly? to)
        {
            var result = await _statisticsService.GetAverageDailyExpenseAsync(from, to);
            return Ok(result);
        }

        [HttpGet("period-over-period-change")]
        public async Task<ActionResult<PeriodChangeDto>> GetPeriodOverPeriodChange([FromQuery] DateOnly currentPeriodStart, [FromQuery] DateOnly currentPeriodEnd)
        {
            var result = await _statisticsService.GetPeriodOverPeriodChangeAsync(currentPeriodStart, currentPeriodEnd);
            return Ok(result);
        }
    }
}
