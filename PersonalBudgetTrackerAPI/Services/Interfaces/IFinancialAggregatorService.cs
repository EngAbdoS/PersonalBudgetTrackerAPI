using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.DailySnapshot;
using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.FinanialRulesBaseDTOs;

namespace PersonalBudgetTrackerAPI.Services.Interfaces
{
    public interface IFinancialAggregatorService
    {
        Task<IEnumerable<UserDailySnapshotDto>> GetDailySnapshotsAsync( DateOnly from, DateOnly? to = null);


        Task<decimal> AggregiateTotalExpense(RuleAggregiationInputDTO inputDTO);

    }
}
