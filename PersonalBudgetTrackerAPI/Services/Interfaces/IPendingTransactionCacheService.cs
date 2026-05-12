using PersonalBudgetTrackerAPI.DTOs.Entities.TransactionDTOs;

namespace PersonalBudgetTrackerAPI.Services.Interfaces
{
    public interface IPendingTransactionCacheService
    {
        Task<Guid> CacheAsync(CreateTransactionDto dto);
        Task<CreateTransactionDto?> GetAsync(Guid cachedId);
        Task DeleteAsync(Guid cachedId);
    }
}
