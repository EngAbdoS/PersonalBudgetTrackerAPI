using PersonalBudgetTrackerAPI.Common.Pagination;
using PersonalBudgetTrackerAPI.DTOs.Entities.TransactionDTOs;

namespace PersonalBudgetTrackerAPI.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<CreateTransactionResponse> CreateAsync(CreateTransactionDto dto);
        Task<CreateTransactionResponse> ConfirmPendingTransactionAsync(Guid cachedId);
        Task<TransactionDto> GetByIdAsync(Guid id);

        Task<PagedResult<TransactionDto>> GetUserTransactionsAsync(TransactionFilterDto filter);

        Task<TransactionRequirementsDto> GetRequirementsAsync();

        Task<List<TransactionSimpleDto>> GetByReasonIdAsync(Guid reasonId);
        Task<List<TransactionSimpleDto>> GetByCategoryIdAsync(Guid categoryId);
        Task<List<TransactionSimpleDto>> GetByPaymentGatewayIdAsync(Guid gatewayId);
        Task<List<TransactionSimpleDto>> GetByPartnerIdAsync(Guid partnerId);
    }
}
