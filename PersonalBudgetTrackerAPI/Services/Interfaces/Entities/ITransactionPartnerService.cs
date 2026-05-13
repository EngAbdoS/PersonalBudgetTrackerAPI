using PersonalBudgetTrackerAPI.Common.Pagination;
using PersonalBudgetTrackerAPI.DTOs.Entities.TransactionPartnerDTOs;

namespace PersonalBudgetTrackerAPI.Services.Interfaces.Entities
{
    public interface ITransactionPartnerService
    {
        Task<TransactionPartnerDto> CreateAsync(CreateTransactionPartnerDto dto);
        Task<TransactionPartnerDto> UpdateAsync(Guid id, UpdateTransactionPartnerDto dto);
        Task DeleteAsync(Guid id);

        Task<bool> TransactionPartnerValidAndExist(Guid id);

        Task<PagedResult<TransactionPartnerDto>> GetAllAsync(int page, int pageSize);

        Task<TransactionPartnerDetailsDto> GetDetailsAsync(Guid id);


        Task<List<TransactionPartnerDto>> GetPartnersByReasonIdAsync(Guid reasonId);

        Task<List<TransactionPartnerDto>> GetPartnersByCategoryIdAsync(Guid categoryId);

    }
}
