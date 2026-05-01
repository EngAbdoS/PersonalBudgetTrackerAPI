using FluentValidation;

namespace PersonalBudgetTrackerAPI.DTOs.Entities.CategoryDTOs
{
    public class CreateCategoryDto
    {
        public string Title { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public bool IsNeedful { get; set; }
        public decimal NeedPriority { get; set; }
    }

    public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
    {
        public CreateCategoryDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Category title is required")
                .MaximumLength(100)
                .WithMessage("Category title must not exceed 100 characters");

            RuleFor(x => x.Details)
                .MaximumLength(500)
                .WithMessage("Category details must not exceed 500 characters");

            RuleFor(x => x.IsNeedful)
                .NotNull()
                .WithMessage("IsNeedful flag is required");

            RuleFor(x => x.NeedPriority)
                .InclusiveBetween(0, 10)
                .WithMessage("NeedPriority must be between 0 and 10")
                .When(x => x.IsNeedful);
        }
    }
}
