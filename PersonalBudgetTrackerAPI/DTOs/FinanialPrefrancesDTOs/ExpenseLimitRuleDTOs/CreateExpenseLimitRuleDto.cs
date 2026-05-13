using FluentValidation;
using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.FinanialRuleDTOs;
using PersonalBudgetTrackerAPI.Models.FinancialPrefrances;

namespace PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.ExpenseLimitRuleDTOs
{
    public class CreateExpenseLimitRuleDto: CreateFinancialRuleBaseDto
    {
        public LimitTargetType TargetType { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? TransactionPartnerId { get; set; }
    }

    public class CreateExpenseLimitRuleDtoValidator : AbstractValidator<CreateExpenseLimitRuleDto>
    {
        public CreateExpenseLimitRuleDtoValidator()
        {
            Include(new CreateFinancialRuleBaseDtoValidator<CreateExpenseLimitRuleDto>());

            RuleFor(x => x.TargetType)
                .IsInEnum()
                .WithMessage("TargetType must be a valid type (Category, TransactionPartner, or All)");

            When(x => x.TargetType == LimitTargetType.Category, () =>
            {
                RuleFor(x => x.CategoryId)
                    .NotNull()
                    .WithMessage("CategoryId is required when TargetType is Category")
                    .Must(id => id.HasValue && id.Value != Guid.Empty)
                    .WithMessage("CategoryId must be a valid non-empty GUID");

                RuleFor(x => x.TransactionPartnerId)
                    .Null()
                    .WithMessage("TransactionPartnerId must not be provided when TargetType is Category");
            });

            When(x => x.TargetType == LimitTargetType.TransactionPartner, () =>
            {
                RuleFor(x => x.TransactionPartnerId)
                    .NotNull()
                    .WithMessage("TransactionPartnerId is required when TargetType is TransactionPartner")
                    .Must(id => id.HasValue && id.Value != Guid.Empty)
                    .WithMessage("TransactionPartnerId must be a valid non-empty GUID");

                RuleFor(x => x.CategoryId)
                    .Null()
                    .WithMessage("CategoryId must not be provided when TargetType is TransactionPartner");
            });

     
        }
    }
}

