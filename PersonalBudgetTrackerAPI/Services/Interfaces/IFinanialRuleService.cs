using PersonalBudgetTrackerAPI.Common.Pagination;
using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.ExpenseLimitRuleDTOs;
using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.FinanialRuleDTOs;
using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.FinanialRulesBaseDTOs;
using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.MinimumBalanceRuleDTOs;
using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.SavingRuleDTOs;

namespace PersonalBudgetTrackerAPI.Services.Interfaces
{
    public interface IFinanialRuleService
    {
        Task<ExpenseLimitRuleDto> CreateExpenseLimitRuleAsync(
          CreateExpenseLimitRuleDto dto);

        Task<MinimumBalanceRuleDto> CreateMinimumBalanceRuleAsync(
            CreateMinimumBalanceRuleDto dto);

        Task<SavingRuleDto> CreateSavingRuleAsync(
            CreateSavingRuleDto dto);

        Task<ExpenseLimitRuleDto> UpdateExpenseLimitRuleAsync(
            Guid id,
            UpdateExpenseLimitRuleDto dto);

        Task<MinimumBalanceRuleDto> UpdateMinimumBalanceRuleAsync(
            Guid id,
            UpdateMinimumBalanceRuleDto dto);

        Task<SavingRuleDto> UpdateSavingRuleAsync(
            Guid id,
            UpdateSavingRuleDto dto);


        Task<List<SavingGoalStatusDto>> GetSavingGoalsStatusAsync();

        Task<PagedResult<FinancialRuleBaseDto>> GetUserRulesAsync(
        FinancialRuleFilterDto filter);

        Task DeactivateRuleAsync(Guid id);

        Task ActivateRuleAsync(Guid id);


        Task<bool> RuleExistsAndActiveAsync(Guid ruleId);


    }
}
