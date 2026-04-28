using PersonalBudgetTrackerAPI.Common;
using PersonalBudgetTrackerAPI.DatabaseContext;
using PersonalBudgetTrackerAPI.DTOs.Entities.ReasonDTOs;
using PersonalBudgetTrackerAPI.Models.Entities;
using PersonalBudgetTrackerAPI.Services.Interfaces;

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

        public Task<List<ReasonDetailsDto>> GetReasonsWithDetailsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<ReasonDto>> GetUserReasonsAsync(int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<List<ReasonDto>> SearchReasonsAsync(string query)
        {
            throw new NotImplementedException();
        }
    }
}
