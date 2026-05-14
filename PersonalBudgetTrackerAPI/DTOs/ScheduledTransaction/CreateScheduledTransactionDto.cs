using FluentValidation;
using PersonalBudgetTrackerAPI.Models.Entities;
using PersonalBudgetTrackerAPI.Models.FinancialPrefrances;

namespace PersonalBudgetTrackerAPI.DTOs.Entities.ScheduledTransactionDTOs
{
    public class CreateScheduledTransactionDto
    {
        public string Title { get; set; } = string.Empty;
        public string? TransactionDetails { get; set; }
        public decimal Amount { get; set; }
        public bool IsIncome { get; set; }
        public bool IsFlexibleAmount { get; set; }
        public PaymentType PaymentType { get; set; }

        public Guid PaymentGatewayId { get; set; }
        public Guid TransactionPartnerId { get; set; }

        // Expense only
        public Guid? CategoryId { get; set; }
        public decimal? FeeAmount { get; set; }

        // Income only
        public Guid? ReasonId { get; set; }

        // Schedule
        public PeriodType PeriodType { get; set; }
        public DayOfWeek? DayOfWeek { get; set; }
        public int? DayOfMonth { get; set; }
    }

    public class CreateScheduledTransactionDtoValidator : AbstractValidator<CreateScheduledTransactionDto>
    {
        public CreateScheduledTransactionDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

            RuleFor(x => x.TransactionDetails)
                .MaximumLength(1000).WithMessage("Details must not exceed 1000 characters.")
                .When(x => x.TransactionDetails is not null);

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0.");

            RuleFor(x => x.PaymentType)
                .IsInEnum().WithMessage("PaymentType must be Cash or Digital.");

            RuleFor(x => x.PaymentGatewayId)
                .NotEmpty().WithMessage("PaymentGatewayId is required.");

            RuleFor(x => x.TransactionPartnerId)
                .NotEmpty().WithMessage("TransactionPartnerId is required.");

            RuleFor(x => x.PeriodType)
                .IsInEnum().WithMessage("PeriodType must be Daily, Weekly, or Monthly.")
                .Must(p => p == PeriodType.Daily || p == PeriodType.Weekly || p == PeriodType.Monthly)
                .WithMessage("PeriodType must be Daily, Weekly, or Monthly.");

            // Weekly — DayOfWeek required
            When(x => x.PeriodType == PeriodType.Weekly, () =>
            {
                RuleFor(x => x.DayOfWeek)
                    .NotNull().WithMessage("DayOfWeek is required when PeriodType is Weekly.")
                    .IsInEnum().WithMessage("DayOfWeek must be a valid day.");

                RuleFor(x => x.DayOfMonth)
                    .Null().WithMessage("DayOfMonth must not be provided when PeriodType is Weekly.");
            });

            // Monthly — DayOfMonth required
            When(x => x.PeriodType == PeriodType.Monthly, () =>
            {
                RuleFor(x => x.DayOfMonth)
                    .NotNull().WithMessage("DayOfMonth is required when PeriodType is Monthly.")
                    .InclusiveBetween(1, 31).WithMessage("DayOfMonth must be between 1 and 31.");

                RuleFor(x => x.DayOfWeek)
                    .Null().WithMessage("DayOfWeek must not be provided when PeriodType is Monthly.");
            });

            // Daily — neither required
            When(x => x.PeriodType == PeriodType.Daily, () =>
            {
                RuleFor(x => x.DayOfWeek)
                    .Null().WithMessage("DayOfWeek must not be provided when PeriodType is Daily.");

                RuleFor(x => x.DayOfMonth)
                    .Null().WithMessage("DayOfMonth must not be provided when PeriodType is Daily.");
            });

            // Expense rules
            When(x => !x.IsIncome, () =>
            {
                RuleFor(x => x.CategoryId)
                    .NotNull().WithMessage("CategoryId is required for expense scheduled transactions.")
                    .Must(id => id.HasValue && id.Value != Guid.Empty)
                    .WithMessage("CategoryId must be a valid non-empty GUID.");

                RuleFor(x => x.FeeAmount)
                    .NotNull().WithMessage("FeeAmount is required for expense scheduled transactions.")
                    .GreaterThanOrEqualTo(0).WithMessage("FeeAmount must be 0 or greater.");

                RuleFor(x => x.ReasonId)
                    .Null().WithMessage("ReasonId must not be provided for expense scheduled transactions.");
            });

            // Income rules
            When(x => x.IsIncome, () =>
            {
                RuleFor(x => x.ReasonId)
                    .NotNull().WithMessage("ReasonId is required for income scheduled transactions.")
                    .Must(id => id.HasValue && id.Value != Guid.Empty)
                    .WithMessage("ReasonId must be a valid non-empty GUID.");

                RuleFor(x => x.CategoryId)
                    .Null().WithMessage("CategoryId must not be provided for income scheduled transactions.");

                RuleFor(x => x.FeeAmount)
                    .Null().WithMessage("FeeAmount must not be provided for income scheduled transactions.");
            });
        }
    }
}