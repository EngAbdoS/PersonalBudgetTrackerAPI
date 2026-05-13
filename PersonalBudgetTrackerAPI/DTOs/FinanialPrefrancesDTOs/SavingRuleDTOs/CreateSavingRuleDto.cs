using FluentValidation;
using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.FinanialRuleDTOs;

namespace PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.SavingRuleDTOs
{
    public class CreateSavingRuleDto : CreateFinancialRuleBaseDto
    {
    }

    public class CreateSavingRuleDtoValidator : AbstractValidator<CreateSavingRuleDto>
    {
        public CreateSavingRuleDtoValidator()
        {
            Include(new CreateFinancialRuleBaseDtoValidator<CreateSavingRuleDto>());
        }
    }
}

