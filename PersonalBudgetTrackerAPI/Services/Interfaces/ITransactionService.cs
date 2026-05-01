using PersonalBudgetTrackerAPI.Common;
using PersonalBudgetTrackerAPI.DTOs.Entities.TransactionDTOs;

namespace PersonalBudgetTrackerAPI.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionDto> CreateAsync(CreateTransactionDto dto);

        Task<TransactionDto> GetByIdAsync(Guid id);

        Task<PagedResult<TransactionDto>> GetUserTransactionsAsync(TransactionFilterDto filter);

        Task<List<TransactionSimpleDto>> GetByReasonIdAsync(Guid reasonId);
        Task<List<TransactionSimpleDto>> GetByCategoryIdAsync(Guid categoryId);
        Task<List<TransactionSimpleDto>> GetByPaymentGatewayIdAsync(Guid gatewayId);
        Task<List<TransactionSimpleDto>> GetByPartnerIdAsync(Guid partnerId);
    }
}
