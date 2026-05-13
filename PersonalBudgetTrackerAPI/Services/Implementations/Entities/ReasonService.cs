using Microsoft.EntityFrameworkCore;
using PersonalBudgetTrackerAPI.Common.Exceptions;
using PersonalBudgetTrackerAPI.Common.Pagination;
using PersonalBudgetTrackerAPI.DatabaseContext;
using PersonalBudgetTrackerAPI.DTOs.Entities.ReasonDTOs;
using PersonalBudgetTrackerAPI.Models.Entities;
using PersonalBudgetTrackerAPI.Services.Interfaces.Entities;

namespace PersonalBudgetTrackerAPI.Services.Implementations.Entities
{
    public class ReasonService : IReasonService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITransactionPartnerService _transactionPartnerService;

        public ReasonService(ApplicationDbContext context, ITransactionPartnerService transactionPartnerService)
        {
            _context = context;
            _transactionPartnerService = transactionPartnerService;
        }


        public async Task<ReasonDto> CreateReasonAsync(string ReasonDetails)
        {
            var reason = new Reason
            {
                Id = Guid.NewGuid(),
                ReasonDetails = ReasonDetails
            };
            _context.Reason.Add(reason);
            await _context.SaveChangesAsync();
            return new ReasonDto
            {
                ReasonId = reason.Id,
                ReasonDetails = reason.ReasonDetails
            };
        }

        public async Task<bool> ReasonValidAndExist(Guid reasonId)
        {
            return await _context.Reason
                .AnyAsync(r => r.Id == reasonId && !r.IsDeleted);
        }

        public async Task<PagedResult<ReasonDetailsDto>> GetReasonsWithDetailsAsync(PaginationQuery pagination)
        {

            var query = _context.Set<Income>()
               .GroupBy(i => new { i.ReasonId, i.Reason.ReasonDetails })
               .Select(g => new ReasonDetailsDto
               {
                   ReasonId = g.Key.ReasonId,
                   ReasonDetails = g.Key.ReasonDetails,
                   UsageCount = g.Count(),
                   TotalIncome = g.Sum(i => i.Amount)
               }).OrderByDescending(x => x.UsageCount);

            var totalCount = await query.CountAsync();

            if (totalCount < 1)
            {
                throw new NotFoundException("there is no any used reasons");

            }

            var items = await query
                              .Skip((pagination.Page - 1) * pagination.PageSize)
                              .Take(pagination.PageSize)
                              .ToListAsync();

            var result = new List<ReasonDetailsDto>();

            foreach (var item in items)
            {
                var partners = await _transactionPartnerService
                    .GetPartnersByReasonIdAsync(item.ReasonId);

                result.Add(new ReasonDetailsDto
                {
                    ReasonId = item.ReasonId,
                    ReasonDetails = item.ReasonDetails,
                    UsageCount = item.UsageCount,
                    TotalIncome = item.TotalIncome,
                    TransactionPartners = partners
                });
            }

            return new PagedResult<ReasonDetailsDto>
            {
                Items = result,
                TotalCount = totalCount,
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };

        }

        public async Task<PagedResult<ReasonDto>> GetUserReasonsAsync(PaginationQuery pagination)
        {

            var query = _context.Reason
                .OrderByDescending(r => r.Id)
                .Select(r => new ReasonDto
                {
                    ReasonId = r.Id,
                    ReasonDetails = r.ReasonDetails
                });

            var totalCount = await query.CountAsync();

            if (totalCount < 1)
            {
                throw new NotFoundException("there is no any used reasons");
            }
            var items = await query
                              .Skip((pagination.Page - 1) * pagination.PageSize)
                              .Take(pagination.PageSize)
                              .ToListAsync();

            return new PagedResult<ReasonDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };


        }

        public async Task<PagedResult<ReasonDto>> SearchReasonsAsync(string queryText, PaginationQuery pagination)
        {
            var query = _context.Reason
            .Where(r => r.ReasonDetails.ToLower().Contains(queryText.ToLower()))
            .OrderByDescending(r => r.Id)
            .Select(r => new ReasonDto
            {
                ReasonId = r.Id,
                ReasonDetails = r.ReasonDetails
            });

            var totalCount = await query.CountAsync();

            if (totalCount < 1)
            {
                throw new NotFoundException("reason not found");

            }

            var items = await query
                                 .Skip((pagination.Page - 1) * pagination.PageSize)
                                 .Take(pagination.PageSize)
                                 .ToListAsync();

            return new PagedResult<ReasonDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };

        }
    }
}
