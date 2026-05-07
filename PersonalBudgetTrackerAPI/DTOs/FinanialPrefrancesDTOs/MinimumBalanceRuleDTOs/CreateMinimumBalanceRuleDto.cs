using FluentValidation;
using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.FinanialRuleDTOs;

namespace PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.MinimumBalanceRuleDTOs
{
    public class CreateMinimumBalanceRuleDto : CreateFinancialRuleBaseDto
    {
    }

    public class CreateMinimumBalanceRuleDtoValidator : AbstractValidator<CreateMinimumBalanceRuleDto>
    {
        public CreateMinimumBalanceRuleDtoValidator()
        {
            Include(new CreateFinancialRuleBaseDtoValidator<CreateMinimumBalanceRuleDto>());
        }
    }
}

