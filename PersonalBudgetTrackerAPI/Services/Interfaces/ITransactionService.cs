using PersonalBudgetTrackerAPI.Common;
using PersonalBudgetTrackerAPI.DTOs.Entities.TransactionDTOs;

namespace PersonalBudgetTrackerAPI.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionDto> CreateAsync(CreateTransactionDto dto);

        Task<PagedResult<TransactionDto>> GetUserTransactionsAsync(TransactionFilterDto filter);

        Task<List<TransactionDto>> GetByReasonIdAsync(Guid reasonId);
        Task<List<TransactionDto>> GetByCategoryIdAsync(Guid categoryId);
        Task<List<TransactionDto>> GetByPaymentGatewayIdAsync(Guid gatewayId);
        Task<List<TransactionDto>> GetByPartnerIdAsync(Guid partnerId);
    }
}
