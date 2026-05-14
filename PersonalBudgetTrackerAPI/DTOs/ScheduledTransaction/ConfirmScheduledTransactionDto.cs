using FluentValidation;

namespace PersonalBudgetTrackerAPI.DTOs.Entities.ScheduledTransactionDTOs
{
    public class ConfirmScheduledTransactionDto
    {
        public decimal? Amount { get; set; } 
    }

    public class ConfirmScheduledTransactionDtoValidator : AbstractValidator<ConfirmScheduledTransactionDto>
    {
        public ConfirmScheduledTransactionDtoValidator()
        {
            When(x => x.Amount.HasValue, () =>
            {
                RuleFor(x => x.Amount)
                    .GreaterThan(0).WithMessage("Amount must be greater than 0.");
            });
        }
    }
}