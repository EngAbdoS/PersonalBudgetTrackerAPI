using FluentValidation;

namespace PersonalBudgetTrackerAPI.DTOs.Auth
{
    public class TokenModelDto
    {
        public string? Token { get; set; } = string.Empty;
        public string? RefreshToken { get; set; } = string.Empty;
    }

    public class  TokenModelDtoValidator : FluentValidation.AbstractValidator<TokenModelDto>
    {

        public TokenModelDtoValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty();
            RuleFor(x => x.RefreshToken)
                .NotEmpty();
        }

    }

}
