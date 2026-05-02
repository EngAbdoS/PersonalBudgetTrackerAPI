using Microsoft.EntityFrameworkCore;
using PersonalBudgetTrackerAPI.Common.Exceptions;
using PersonalBudgetTrackerAPI.Common.Pagination;
using PersonalBudgetTrackerAPI.DatabaseContext;
using PersonalBudgetTrackerAPI.DTOs.Entities.CategoryDTOs;
using PersonalBudgetTrackerAPI.Models.Entities;
using PersonalBudgetTrackerAPI.Services.Interfaces;

namespace PersonalBudgetTrackerAPI.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITransactionPartnerService _transactionPartnerService;

        public CategoryService(ApplicationDbContext context, ITransactionPartnerService transactionPartnerService)
        {
            _context = context;
            _transactionPartnerService = transactionPartnerService;
        }
        public async Task<bool> CategoryValidAndExist(Guid categoryId)
        {
            return await _context.Category
                .AnyAsync(c => c.Id == categoryId && !c.IsDeleted);
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
        {
            var category = new Category
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Details = dto.Details,
                IsNeedful = dto.IsNeedful,
                NeedPriority = dto.NeedPriority
            };

            _context.Category.Add(category);
            await _context.SaveChangesAsync();

            return category.ToDto();
        }

        public async Task<CategoryDto> UpdateCategoryAsync(Guid id, UpdateCategoryDto dto)
        {
            var category = await _context.Category.FindAsync(id)
                ?? throw new NotFoundException("Category not found");

            category.Title = dto.Title;
            category.Details = dto.Details;
            category.IsNeedful = dto.IsNeedful;
            category.NeedPriority = dto.NeedPriority;

            await _context.SaveChangesAsync();

            return category.ToDto();
        }

        public async Task DeleteCategoryAsync(Guid id)
        {
            var category = await _context.Category.FindAsync(id)
                ?? throw new NotFoundException("Category not found");

            var hasTransactions = await _context.Set<Expense>()
                .AnyAsync(e => e.CategoryId == id);

            if (hasTransactions)
                throw new BadRequestException("Cannot delete category with related transactions");

            _context.Category.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<CategoryDetailsDto> GetCategoryByIdAsync(Guid id)
        {
            var categoryDetails = await _context.Set<Expense>()
                .Where(e => e.CategoryId == id)
                .GroupBy(e => new { e.CategoryId, e.Category.Title, e.Category.Details, e.Category.IsNeedful, e.Category.NeedPriority })
                .Select(g => new CategoryDetailsDto
                {
                    CategoryId = g.Key.CategoryId,
                    Title = g.Key.Title,
                    Details = g.Key.Details,
                    IsNeedful = g.Key.IsNeedful,
                    NeedPriority = g.Key.NeedPriority,
                    UsageCount = g.Count(),
                    TotalExpense = g.Sum(e => e.Amount),
                })
                .FirstOrDefaultAsync();

            if (categoryDetails == null)
                throw new NotFoundException("Category not found or has no expenses");

            var partners = await _transactionPartnerService
                .GetPartnersByCategoryIdAsync(categoryDetails.CategoryId);

            categoryDetails.TransactionPartners = partners;

            return categoryDetails;
        }


        public async Task<PagedResult<CategoryDto>> GetCategoriesAsync(PaginationQuery pagination)
        {
            var query = _context.Category
                .OrderByDescending(c => c.NeedPriority)
                .Select(c => c.ToDto());

            var total = await query.CountAsync();

            if (total < 1)
            {
                throw new NotFoundException("there is no any categories");
            }

            var items = await query
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return new PagedResult<CategoryDto>
            {
                Items = items,
                TotalCount = total,
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };
        }

        public async Task<PagedResult<CategoryDto>> SearchCategoriesAsync(
         string? search,
         bool? isNeedful,
         decimal? minPriority,
         decimal? maxPriority,
         PaginationQuery pagination)
        {
            var query = _context.Category.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c =>
                    EF.Functions.Like(c.Title, $"%{search}%") ||
                    EF.Functions.Like(c.Details, $"%{search}%"));
            }

            if (isNeedful.HasValue)
                query = query.Where(c => c.IsNeedful == isNeedful);

            if (minPriority.HasValue)
                query = query.Where(c => c.NeedPriority >= minPriority);

            if (maxPriority.HasValue)
                query = query.Where(c => c.NeedPriority <= maxPriority);

            query = query.OrderByDescending(c => c.NeedPriority);

            var total = await query.CountAsync();

            if (total < 1)
            {
                throw new NotFoundException("no categories found");
            }
            var items = await query
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .Select(c => c.ToDto())
                .ToListAsync();

            return new PagedResult<CategoryDto>
            {
                Items = items,
                TotalCount = total,
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };
        }

    }
}
