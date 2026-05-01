using PersonalBudgetTrackerAPI.Common;
using PersonalBudgetTrackerAPI.DTOs.Entities.TransactionDTOs;
using PersonalBudgetTrackerAPI.Services.Interfaces;

namespace PersonalBudgetTrackerAPI.Services.Implementations
{
    public class TransactionService : ITransactionService
    {
        public Task<TransactionDto> CreateAsync(CreateTransactionDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<List<TransactionDto>> GetByCategoryIdAsync(Guid categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<List<TransactionDto>> GetByPartnerIdAsync(Guid partnerId)
        {
            throw new NotImplementedException();
        }

        public Task<List<TransactionDto>> GetByPaymentGatewayIdAsync(Guid gatewayId)
        {
            throw new NotImplementedException();
        }

        public Task<List<TransactionDto>> GetByReasonIdAsync(Guid reasonId)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<TransactionDto>> GetUserTransactionsAsync(TransactionFilterDto filter)
        {
            throw new NotImplementedException();
        }
    }
}
