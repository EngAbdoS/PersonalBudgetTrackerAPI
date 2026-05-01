using FluentValidation;
using PersonalBudgetTrackerAPI.DTOs.Entities.CategoryDTOs;
using PersonalBudgetTrackerAPI.DTOs.Entities.PaymentGatewayDtos;
using PersonalBudgetTrackerAPI.DTOs.Entities.TransactionPartnerDTOs;
using PersonalBudgetTrackerAPI.Models.Entities;

namespace PersonalBudgetTrackerAPI.DTOs.Entities.TransactionDTOs
{
    public class CreateTransactionDto
    {
        public decimal Amount { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? TransactionDetails { get; set; }
        public DateTime Date { get; set; }

        public PaymentType PaymentType { get; set; }

        public bool IsIncome { get; set; }

        public Guid? PaymentGatewayId { get; set; }
        public CreatePaymentGatewayDto? NewPaymentGateway { get; set; }

        public Guid? TransactionPartnerId { get; set; }
        public CreateTransactionPartnerDto? NewPartner { get; set; }

        // Income
        public Guid? ReasonId { get; set; }
        public string? NewReason { get; set; }

        // Expense
        public Guid? CategoryId { get; set; }
        public CreateCategoryDto? NewCategory { get; set; }
        public decimal? FeeAmount { get; set; }
    }

    public class CreateTransactionDtoValidator : AbstractValidator<CreateTransactionDto>
    {
        public CreateTransactionDtoValidator()
        {
            // ── Core fields 

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Transaction amount must be greater than 0");

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Transaction title is required")
                .MaximumLength(200)
                .WithMessage("Transaction title must not exceed 200 characters");

            RuleFor(x => x.TransactionDetails)
                .MaximumLength(1000)
                .WithMessage("Transaction details must not exceed 1000 characters")
                .When(x => x.TransactionDetails is not null);

            RuleFor(x => x.Date)
                .NotEmpty()
                .WithMessage("Transaction date is required")
                .LessThanOrEqualTo(DateTime.Now)
                .WithMessage("Transaction date cannot be in the future");

            RuleFor(x => x.PaymentType)
                .IsInEnum()
                .WithMessage("Payment type must be a valid type");

            // ── Payment Gateway

            RuleFor(x => x)
                .Must(x =>
                {
                    var hasId = x.PaymentGatewayId.HasValue && x.PaymentGatewayId != Guid.Empty;
                    var hasNew = x.NewPaymentGateway is not null;
                    return hasId ^ hasNew;                     
                })
                .WithName("PaymentGateway")
                .WithMessage("Provide either an existing PaymentGatewayId or a new payment gateway — not both and not neither");

            When(x => x.NewPaymentGateway is not null, () =>
            {
                RuleFor(x => x.NewPaymentGateway!)
                    .SetValidator(new CreatePaymentGatewayDtoValidator());
            });

            // ── Transaction Partner 

            RuleFor(x => x)
                .Must(x =>
                {
                    var hasId = x.TransactionPartnerId.HasValue && x.TransactionPartnerId != Guid.Empty;
                    var hasNew = x.NewPartner is not null;
                    return hasId ^ hasNew;                  
                })
                .WithName("TransactionPartner")
                .WithMessage("Provide either an existing TransactionPartnerId or a new partner — not both");

            When(x => x.NewPartner is not null, () =>
            {
                RuleFor(x => x.NewPartner!)
                    .SetValidator(new CreateTransactionPartnerDtoValidator());
            });

            // ── Income rules ─────────────────────────────────────────────────────────

            When(x => x.IsIncome, () =>
            {
                // Exactly one of ReasonId / NewReason
                RuleFor(x => x)
                    .Must(x =>
                    {
                        var hasId = x.ReasonId.HasValue && x.ReasonId != Guid.Empty;
                        var hasNewStr = !string.IsNullOrWhiteSpace(x.NewReason);
                        return hasId ^ hasNewStr;               
                    })
                    .WithName("IncomeReason")
                    .WithMessage("Income transactions require either a ReasonId or a new reason text — not both and not neither");

                // Validate new reason text when supplied
                When(x => !string.IsNullOrWhiteSpace(x.NewReason), () =>
                {
                    RuleFor(x => x.NewReason!)
                        .MaximumLength(200)
                        .WithMessage("Reason text must not exceed 200 characters");
                });

                // Expense-only fields must be absent for income transactions
                RuleFor(x => x.CategoryId)
                    .Null()
                    .WithMessage("CategoryId must not be provided for income transactions");

                RuleFor(x => x.NewCategory)
                    .Null()
                    .WithMessage("NewCategory must not be provided for income transactions");

                RuleFor(x => x.FeeAmount)
                    .Null()
                    .WithMessage("FeeAmount must not be provided for income transactions");
            });

            // ── Expense rules 

            When(x => !x.IsIncome, () =>
            {
                RuleFor(x => x)
                    .Must(x =>
                    {
                        var hasId = x.CategoryId.HasValue && x.CategoryId != Guid.Empty;
                        var hasNew = x.NewCategory is not null;
                        return hasId ^ hasNew;                 
                    })
                    .WithName("ExpenseCategory")
                    .WithMessage("Expense transactions require either a CategoryId or a new category — not both and not neither");

                // Validate new category object when supplied
                When(x => x.NewCategory is not null, () =>
                {
                    RuleFor(x => x.NewCategory!)
                        .SetValidator(new CreateCategoryDtoValidator());
                });

                RuleFor(x => x.FeeAmount)
                  .NotNull()
                  .WithMessage("Fee amount is required for expense transactions")
                  .GreaterThanOrEqualTo(0)
                  .WithMessage("Fee amount must be 0 or greater");

                RuleFor(x => x.ReasonId)
                    .Null()
                    .WithMessage("ReasonId must not be provided for expense transactions");

                RuleFor(x => x.NewReason)
                    .Null()
                    .WithMessage("NewReason must not be provided for expense transactions");
            });
        }

       
    }
}