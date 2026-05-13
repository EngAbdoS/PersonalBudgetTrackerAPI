using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.DailySnapshot;
using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.FinanialRulesBaseDTOs;

namespace PersonalBudgetTrackerAPI.Services.Interfaces.FinanialPrefrances
{
    public interface IFinancialAggregatorService
    {
        Task<IEnumerable<UserDailySnapshotDto>> GetDailySnapshotsAsync( DateOnly from, DateOnly? to = null);


        Task<decimal> AggregiateTotalExpense(RuleAggregiationInputDTO inputDTO);

        Task<decimal> AggregiateTotalIncome(DateOnly from, Guid? PaymentGatewayId , DateOnly? to = null);

        Task<decimal> AggregateTotalBalance( DateOnly from,Guid? paymentGatewayId, DateOnly? to = null);

    }
}
