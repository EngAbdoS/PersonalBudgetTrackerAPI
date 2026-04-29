using PersonalBudgetTrackerAPI.Common;
using PersonalBudgetTrackerAPI.DTOs.Entities.CategoryDTOs;

namespace PersonalBudgetTrackerAPI.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto);
        Task<CategoryDto> UpdateCategoryAsync(Guid id, UpdateCategoryDto dto);
        Task DeleteCategoryAsync(Guid id);

        Task<bool> CategoryValidAndExist(Guid categoryId);

        Task<CategoryDto> GetCategoryByIdAsync(Guid id);

        Task<PagedResult<CategoryDto>> GetCategoriesAsync(int page, int pageSize);

        Task<PagedResult<CategoryDto>> SearchCategoriesAsync(
            string? search,
            bool? isNeedful,
            decimal? minPriority,
            decimal? maxPriority,
            int page,
            int pageSize);

        Task<PagedResult<CategoryDetailsDto>> GetCategoriesWithDetailsAsync(int page, int pageSize);

    }
}
