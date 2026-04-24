using FluentValidation;

namespace PersonalBudgetTrackerAPI.DTOs.Auth
{
    public class LoginDto
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class LoginDtoValidator : FluentValidation.AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .MinimumLength(4);
            RuleFor(x => x.Password)
                  .NotEmpty()
                  .MinimumLength(6)
                  .Matches("[A-Z]").WithMessage("Must contain uppercase")
                  .Matches("[0-9]").WithMessage("Must contain digit")
                  .Matches("[^a-zA-Z0-9]").WithMessage("Must contain special character");
        }
    }

}
