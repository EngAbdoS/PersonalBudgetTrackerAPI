using FluentValidation;
using PersonalBudgetTrackerAPI.Models.Entities;

namespace PersonalBudgetTrackerAPI.DTOs.Entities.PaymentGatewayDtos
{
    public class CreatePaymentGatewayDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public PaymentGatewayType PaymentGatewayType { get; set; }
        public decimal InitialBalance { get; set; }
        public DateTime ExpirationDate { get; set; }
    }

    public class CreatePaymentGatewayDtoValidator : AbstractValidator<CreatePaymentGatewayDto>
    {
        public CreatePaymentGatewayDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Payment gateway title is required")
                .MaximumLength(100)
                .WithMessage("Payment gateway title must not exceed 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .WithMessage("Payment gateway description must not exceed 500 characters");

            RuleFor(x => x.BankName)
                .MaximumLength(100)
                .WithMessage("Bank name must not exceed 100 characters");

            RuleFor(x => x.PaymentGatewayType)
                .IsInEnum()
                .WithMessage("Payment gateway type must be a valid type");

            RuleFor(x => x.InitialBalance)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Initial balance must be greater than or equal to 0");

            RuleFor(x => x.ExpirationDate)
                .GreaterThan(DateTime.Now)
                .WithMessage("Expiration date must be in the future");
        }
    }
}
