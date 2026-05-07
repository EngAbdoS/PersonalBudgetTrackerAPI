using FluentValidation;
using PersonalBudgetTrackerAPI.Models.FinancialPrefrances;

namespace PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.FinanialRuleDTOs
{
    public abstract class CreateFinancialRuleBaseDto
    {
        public required string Title { get; set; }
        public string? Notes { get; set; }
        public Models.FinancialPrefrances.ValueType ValueType { get; set; }
        public decimal Value { get; set; }
        public ScopeType ScopeType { get; set; }
        public Guid? PaymentGatewayId { get; set; }
        public PeriodType PeriodType { get; set; }
        public DateTime? PeriodStart { get; set; }
        public DateTime? PeriodEnd { get; set; }
        public RecurrenceMode RecurrenceMode { get; set; }
        public PeriodType? RecurrencePeriod { get; set; }
    }

    public class CreateFinancialRuleBaseDtoValidator<T> : AbstractValidator<T>
        where T : CreateFinancialRuleBaseDto
    {
        public CreateFinancialRuleBaseDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Rule title is required")
                .MaximumLength(200)
                .WithMessage("Rule title must not exceed 200 characters");

            RuleFor(x => x.Notes)
                .MaximumLength(1000)
                .WithMessage("Notes must not exceed 1000 characters")
                .When(x => x.Notes is not null);

            RuleFor(x => x.ValueType)
                .IsInEnum()
                .WithMessage("ValueType must be a valid type (Percentage or StaticAmount)");

            RuleFor(x => x.Value)
                .GreaterThan(0)
                .WithMessage("Value must be greater than 0");

            When(x => x.ValueType == Models.FinancialPrefrances.ValueType.Percentage, () =>
            {
                RuleFor(x => x.Value)
                    .InclusiveBetween(0, 100)
                    .WithMessage("Percentage value must be between 0 and 100");
            });

            RuleFor(x => x.ScopeType)
                .IsInEnum()
                .WithMessage("ScopeType must be a valid type (TotalUser or SpecificGateway)");

            When(x => x.ScopeType == ScopeType.SpecificGateway, () =>
            {
                RuleFor(x => x.PaymentGatewayId)
                    .NotNull()
                    .WithMessage("PaymentGatewayId is required when ScopeType is SpecificGateway")
                    .Must(id => id.HasValue && id.Value != Guid.Empty)
                    .WithMessage("PaymentGatewayId must be a valid non-empty GUID");
            });

            When(x => x.ScopeType == ScopeType.TotalUser, () =>
            {
                RuleFor(x => x.PaymentGatewayId)
                    .Null()
                    .WithMessage("PaymentGatewayId must not be provided when ScopeType is TotalUser");
            });

            RuleFor(x => x.PeriodType)
                .IsInEnum()
                .WithMessage("PeriodType must be a valid type (Daily, Weekly, Monthly, Quarterly, Yearly, or Custom)");

            When(x => x.PeriodType == PeriodType.Custom, () =>
            {
                RuleFor(x => x.PeriodStart)
                    .NotNull()
                    .WithMessage("PeriodStart is required when PeriodType is Custom");

                RuleFor(x => x.PeriodEnd)
                    .NotNull()
                    .WithMessage("PeriodEnd is required when PeriodType is Custom");

                RuleFor(x => x.PeriodEnd)
                    .GreaterThan(x => x.PeriodStart)
                    .WithMessage("PeriodEnd must be after PeriodStart")
                    .When(x => x.PeriodStart.HasValue && x.PeriodEnd.HasValue);
            });

            When(x => x.PeriodType != PeriodType.Custom, () =>
            {
                RuleFor(x => x.PeriodStart)
                    .Null()
                    .WithMessage("PeriodStart must not be provided when PeriodType is not Custom");

                RuleFor(x => x.PeriodEnd)
                    .Null()
                    .WithMessage("PeriodEnd must not be provided when PeriodType is not Custom");
            });

            RuleFor(x => x.RecurrenceMode)
                .IsInEnum()
                .WithMessage("RecurrenceMode must be a valid mode (OneTime, Recurring, or Manual)");

            When(x => x.RecurrenceMode == RecurrenceMode.OneTime, () =>
            {
                RuleFor(x => x.RecurrencePeriod)
                    .Null()
                    .WithMessage("RecurrencePeriod must not be provided when RecurrenceMode is OneTime");
            });

            When(x => x.RecurrenceMode == RecurrenceMode.Recurring, () =>
            {
                RuleFor(x => x.RecurrencePeriod)
                    .NotNull()
                    .WithMessage("RecurrencePeriod is required when RecurrenceMode is Recurring")
                    .IsInEnum()
                    .WithMessage("RecurrencePeriod must be a valid period type")
                    .Must(p => p.HasValue && p.Value != PeriodType.Custom)
                    .WithMessage("RecurrencePeriod cannot be Custom — use a standard period (Daily, Weekly, Monthly, Quarterly, or Yearly)");
            });

            When(x => x.RecurrenceMode == RecurrenceMode.Manual && x.RecurrencePeriod.HasValue, () =>
            {
                RuleFor(x => x.RecurrencePeriod)
                    .IsInEnum()
                    .WithMessage("RecurrencePeriod must be a valid period type")
                    .Must(p => p.HasValue && p.Value != PeriodType.Custom)
                    .WithMessage("RecurrencePeriod cannot be Custom");
            });
        }
    }
}

