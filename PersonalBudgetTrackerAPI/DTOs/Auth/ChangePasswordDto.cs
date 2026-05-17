using FluentValidation;

namespace PersonalBudgetTrackerAPI.DTOs.Auth
{
    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }

    public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordDtoValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty();

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .MinimumLength(8)
                .MaximumLength(100)
                .Matches("[A-Z]").WithMessage("Must contain uppercase")
                .Matches("[a-z]").WithMessage("Must contain lowercase")
                .Matches("[0-9]").WithMessage("Must contain digit")
                .Matches("[^a-zA-Z0-9]").WithMessage("Must contain special character")
                .NotEqual(x => x.CurrentPassword).WithMessage("New password must be different from current password.");
        }
    }
}
