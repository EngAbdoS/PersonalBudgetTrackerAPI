using FluentValidation;

namespace PersonalBudgetTrackerAPI.DTOs.Auth
{
    public class RegisterDto
    {
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FullName { get; set; } = string.Empty;
    }

    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .MinimumLength(4);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                  .NotEmpty()
                  .MinimumLength(6)
                  .Matches("[A-Z]").WithMessage("Must contain uppercase")
                  .Matches("[0-9]").WithMessage("Must contain digit")
                  .Matches("[^a-zA-Z0-9]").WithMessage("Must contain special character");

            RuleFor(x => x.FullName)    
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}
