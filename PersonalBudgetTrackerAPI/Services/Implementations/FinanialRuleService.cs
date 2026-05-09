using PersonalBudgetTrackerAPI.Common.Pagination;
using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.ExpenseLimitRuleDTOs;
using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.FinanialRuleDTOs;
using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.FinanialRulesBaseDTOs;
using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.MinimumBalanceRuleDTOs;
using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.SavingRuleDTOs;
using PersonalBudgetTrackerAPI.Services.Interfaces;

namespace PersonalBudgetTrackerAPI.Services.Implementations
{
    public class FinanialRuleService : IFinanialRuleService
    {
        public Task ActivateRuleAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ExpenseLimitRuleDto> CreateExpenseLimitRuleAsync(CreateExpenseLimitRuleDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<MinimumBalanceRuleDto> CreateMinimumBalanceRuleAsync(CreateMinimumBalanceRuleDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<SavingRuleDto> CreateSavingRuleAsync(CreateSavingRuleDto dto)
        {
            throw new NotImplementedException();
        }

        public Task DeactivateRuleAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<ExpenseLimitRuleDto>> GetExpenseLimitRulesAsync(int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<MinimumBalanceRuleDto>> GetMinimumBalanceRulesAsync(int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<SavingRuleDto>> GetSavingGoalRulesAsync(int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<List<SavingGoalStatusDto>> GetSavingGoalsStatusAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> RuleExistsAndActiveAsync(Guid ruleId)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<FinancialRuleBaseDto>> SearchRulesAsync(FinancialRuleSearchFilterDto filter)
        {
            throw new NotImplementedException();
        }

        public Task<ExpenseLimitRuleDto> UpdateExpenseLimitRuleAsync(Guid id, UpdateExpenseLimitRuleDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<MinimumBalanceRuleDto> UpdateMinimumBalanceRuleAsync(Guid id, UpdateMinimumBalanceRuleDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<SavingRuleDto> UpdateSavingRuleAsync(Guid id, UpdateSavingRuleDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
