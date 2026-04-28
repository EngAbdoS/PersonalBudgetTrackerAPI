using Microsoft.EntityFrameworkCore;
using PersonalBudgetTrackerAPI.Common;
using PersonalBudgetTrackerAPI.Common.Exceptions;
using PersonalBudgetTrackerAPI.DatabaseContext;
using PersonalBudgetTrackerAPI.DTOs.Entities.ReasonDTOs;
using PersonalBudgetTrackerAPI.Models.Entities;
using PersonalBudgetTrackerAPI.Services.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PersonalBudgetTrackerAPI.Services.Implementations
{
    public class ReasonService : IReasonService
    {
        private readonly ApplicationDbContext _context;

        public ReasonService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<ReasonDto> CreateReasonAsync(string ReasonDetails) {
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

        public async Task<PagedResult<ReasonDetailsDto>> GetReasonsWithDetailsAsync(int page, int pageSize)
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
                              .Skip((page - 1) * pageSize)
                              .Take(pageSize)
                              .ToListAsync();

            return new PagedResult<ReasonDetailsDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

        }

        public async Task<PagedResult<ReasonDto>> GetUserReasonsAsync(int page, int pageSize)
        {

            var query = _context.Reason
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
                              .Skip((page - 1) * pageSize)
                              .Take(pageSize)
                              .ToListAsync();

            return new PagedResult<ReasonDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };


        }

        public Task<List<ReasonDto>> SearchReasonsAsync(string query)
        {
            throw new NotImplementedException();
        }
    }
}
