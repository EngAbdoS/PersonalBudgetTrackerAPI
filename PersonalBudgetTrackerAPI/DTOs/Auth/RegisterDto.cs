using FluentValidation;

namespace PersonalBudgetTrackerAPI.DTOs.Auth
{
    public class RegisterDto
    {
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FullName { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = null!;

        public string LanguageCode { get; set; } = "en";
        public string CurrencyCode { get; set; } = "EGP";
        public string TimeZone { get; set; } = "Africa/Cairo";

        public bool IsDarkMode { get; set; }

        public bool PushNotificationsEnabled { get; set; } = true;
    }

    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.Username)
                 .NotEmpty()
                 .Length(4, 50)
                 .Matches(@"^(?=.{4,50}$)[a-zA-Z0-9]+([._]?[a-zA-Z0-9]+)*$")
                 .WithMessage("Invalid username");

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(256);

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8)
                .MaximumLength(100)
                .Matches("[A-Z]").WithMessage("Must contain uppercase")
                .Matches("[a-z]").WithMessage("Must contain lowercase")
                .Matches("[0-9]").WithMessage("Must contain digit")
                .Matches("[^a-zA-Z0-9]").WithMessage("Must contain special character");

            RuleFor(x => x.FullName)
                .NotEmpty()
                .Length(2, 100)
                .Matches(@"^[\p{L}\s'-]+$")
                .WithMessage("Invalid full name");

            RuleFor(x => x.PhoneNumber)
             .NotEmpty()
             .Matches(@"^\+[1-9]\d{7,14}$")
             .WithMessage("Phone number must be in international format");


            RuleFor(x => x.LanguageCode)
                .Must(x => ValidationRules.SupportedLanguages.Contains(x))
                .WithMessage("Invalid language code");

            RuleFor(x => x.CurrencyCode)
                .Must(x => ValidationRules.SupportedCurrencies.Contains(x))
                .WithMessage("Invalid currency code");

            RuleFor(x => x.TimeZone).NotEmpty()
                .Must(ValidationRules.BeValidTimeZone)
                .WithMessage("Invalid timezone");
        }


       


    }
}