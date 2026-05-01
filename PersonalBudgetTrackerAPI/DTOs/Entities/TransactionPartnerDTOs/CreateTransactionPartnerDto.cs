using FluentValidation;

namespace PersonalBudgetTrackerAPI.DTOs.Entities.TransactionPartnerDTOs
{
    public class CreateTransactionPartnerDto
    {
        public string Name { get; set; } = string.Empty;
        public string Info { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Contact { get; set; } = string.Empty;
    }

    public class CreateTransactionPartnerDtoValidator : AbstractValidator<CreateTransactionPartnerDto>
    {
        public CreateTransactionPartnerDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Partner name is required")
                .MaximumLength(150)
                .WithMessage("Partner name must not exceed 150 characters");

            RuleFor(x => x.Info)
                .MaximumLength(500)
                .WithMessage("Partner info must not exceed 500 characters");

            RuleFor(x => x.Location)
                .MaximumLength(200)
                .WithMessage("Partner location must not exceed 200 characters");

            RuleFor(x => x.Contact)
                .MaximumLength(100)
                .WithMessage("Partner contact must not exceed 100 characters")
                .Must(BeValidContact)
                .WithMessage("Partner contact must be in a valid format (email or phone)")
                .When(x => !string.IsNullOrEmpty(x.Contact));
        }

        private static bool BeValidContact(string contact)
        {
            if (string.IsNullOrEmpty(contact))
                return true;

            // Simple validation: check if it looks like an email or phone
            var isEmail = contact.Contains("@") && contact.Contains(".");
            var isPhone = contact.All(c => char.IsDigit(c) || c == '+' || c == '-' || c == ' ' || c == '(' || c == ')');

            return isEmail || isPhone;
        }
    }
}
