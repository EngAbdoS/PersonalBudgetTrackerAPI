using PersonalBudgetTrackerAPI.Common;
using PersonalBudgetTrackerAPI.DTOs.Entities.ReasonDTOs;
using PersonalBudgetTrackerAPI.Models.Entities;

namespace PersonalBudgetTrackerAPI.Services.Interfaces
{
    public interface IReasonService
    {

        Task<ReasonDto> CreateReasonAsync(string ReasonDetails); 

        Task<PagedResult<ReasonDto>> GetUserReasonsAsync(int page, int pageSize);

        Task<PagedResult<ReasonDetailsDto>> GetReasonsWithDetailsAsync(int page, int pageSize);

        Task<PagedResult<ReasonDto>> SearchReasonsAsync(string query, int page, int pageSize);

    }
}
