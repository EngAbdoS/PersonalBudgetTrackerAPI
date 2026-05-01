using PersonalBudgetTrackerAPI.Common.Pagination;
using PersonalBudgetTrackerAPI.DTOs.Entities.ReasonDTOs;
using PersonalBudgetTrackerAPI.Models.Entities;

namespace PersonalBudgetTrackerAPI.Services.Interfaces
{
    public interface IReasonService
    {

        Task<bool> ReasonValidAndExist(Guid reasonId);

        Task<ReasonDto> CreateReasonAsync(string ReasonDetails); 

        Task<PagedResult<ReasonDto>> GetUserReasonsAsync(PaginationQuery pagination);

        Task<PagedResult<ReasonDetailsDto>> GetReasonsWithDetailsAsync(PaginationQuery pagination);

        Task<PagedResult<ReasonDto>> SearchReasonsAsync(string query, PaginationQuery pagination);

    }
}
