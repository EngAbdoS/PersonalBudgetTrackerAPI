using FluentValidation;

namespace PersonalBudgetTrackerAPI.DTOs.Auth
{
    public class UpdateProfileDto
    {
        public string? FullName { get; set; } 
        public string? LanguageCode { get; set; } 
        public string? CurrencyCode { get; set; } 
        public string? TimeZone { get; set; } 
        public bool IsDarkMode { get; set; }
        public bool PushNotificationsEnabled { get; set; } 
    }

    public class UpdateProfileDtoValidator : AbstractValidator<UpdateProfileDto>
    {
        public UpdateProfileDtoValidator()
        {
            RuleFor(x => x.FullName)
                .MinimumLength(2)
                .MaximumLength(100)
                .Matches(@"^[\p{L}\s'-]+$")
                .WithMessage("Invalid full name")
                .When(x => !string.IsNullOrWhiteSpace(x.FullName));


            RuleFor(x => x.LanguageCode)
                .Must(x => ValidationRules.SupportedLanguages.Contains(x))
                .WithMessage("Invalid language code").When(x => !string.IsNullOrWhiteSpace(x.LanguageCode)); ;

            RuleFor(x => x.CurrencyCode)
                .Must(x => ValidationRules.SupportedCurrencies.Contains(x))
                .WithMessage("Invalid currency code").When(x => !string.IsNullOrWhiteSpace(x.CurrencyCode));

            RuleFor(x => x.TimeZone)
                .Must(ValidationRules.BeValidTimeZone)
                .WithMessage("Invalid timezone").When(x => !string.IsNullOrWhiteSpace(x.TimeZone));
        }
    }

}
