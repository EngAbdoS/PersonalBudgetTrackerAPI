using PersonalBudgetTrackerAPI.Common.Pagination;
using PersonalBudgetTrackerAPI.DTOs.Entities.CategoryDTOs;

namespace PersonalBudgetTrackerAPI.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto);
        Task<CategoryDto> UpdateCategoryAsync(Guid id, UpdateCategoryDto dto);
        Task DeleteCategoryAsync(Guid id);

        Task<bool> CategoryValidAndExist(Guid categoryId);

        Task<CategoryDetailsDto> GetCategoryByIdAsync(Guid id);

        Task<PagedResult<CategoryDto>> GetCategoriesAsync(PaginationQuery pagination);

        Task<PagedResult<CategoryDto>> SearchCategoriesAsync(
            string? search,
            bool? isNeedful,
            decimal? minPriority,
            decimal? maxPriority,
            PaginationQuery pagination);


    }
}
