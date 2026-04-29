using PersonalBudgetTrackerAPI.Models.Entities;

namespace PersonalBudgetTrackerAPI.DTOs.Entities.CategoryDTOs
{
    public static class CategoryExtensions
    {
        public static CategoryDto ToDto(this Category c)
        {
            return new CategoryDto
            {
                CategoryId = c.Id,
                Title = c.Title,
                Details = c.Details,
                IsNeedful = c.IsNeedful,
                NeedPriority = c.NeedPriority
            };
        }
    }
}
