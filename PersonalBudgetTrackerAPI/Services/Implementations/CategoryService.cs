using Microsoft.EntityFrameworkCore;
using PersonalBudgetTrackerAPI.Common;
using PersonalBudgetTrackerAPI.Common.Exceptions;
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


        public async Task<CategoryDto> GetCategoryByIdAsync(Guid id)
        {
            var category = await _context.Category
                .FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new NotFoundException("Category not found");

            return category.ToDto();
        }



        public async Task<PagedResult<CategoryDto>> GetCategoriesAsync(int page, int pageSize)
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
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<CategoryDto>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PagedResult<CategoryDetailsDto>> GetCategoriesWithDetailsAsync(int page, int pageSize)
        {
            var query = _context.Set<Expense>()
                .GroupBy(e => new { e.CategoryId, e.Category.Title, e.Category.Details })
                .Select( g  => new CategoryDetailsDto
                {
                    CategoryId = g.Key.CategoryId,
                    Title = g.Key.Title,
                    Details = g.Key.Details,
                    UsageCount = g.Count(),
                    TotalExpense = g.Sum(e => e.Amount),
                })
                .OrderByDescending(x => x.UsageCount);

            var total = await query.CountAsync();

            if (total < 1)
            {
                throw new NotFoundException("there is no any categories");
            }

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new List<CategoryDetailsDto>();

            foreach (var item in items)
            {
                var partners = await _transactionPartnerService
                    .GetPartnersByCategoryIdAsync(item.CategoryId);

                result.Add(new CategoryDetailsDto
                {
                    CategoryId = item.CategoryId,
                    Title = item.Title,
                    Details = item.Details,
                    UsageCount = item.UsageCount,
                    TotalExpense = item.TotalExpense,
                    TransactionPartners = partners
                });
            }


            return new PagedResult<CategoryDetailsDto>
            {
                Items = result,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }



        public async Task<PagedResult<CategoryDto>> SearchCategoriesAsync(
         string? search,
         bool? isNeedful,
         decimal? minPriority,
         decimal? maxPriority,
         int page,
         int pageSize)
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
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => c.ToDto())
                .ToListAsync();

            return new PagedResult<CategoryDto>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

    }
}
