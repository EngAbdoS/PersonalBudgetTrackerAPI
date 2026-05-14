using PersonalBudgetTrackerAPI.Common.Pagination;
using PersonalBudgetTrackerAPI.DTOs.Entities.ScheduledTransactionDTOs;
using PersonalBudgetTrackerAPI.DTOs.Entities.TransactionDTOs;
using PersonalBudgetTrackerAPI.DTOs.ScheduledTransaction;

namespace PersonalBudgetTrackerAPI.Services.Interfaces.Entities
{
    public interface IScheduledTransactionService
    {
        Task<ScheduledTransactionDto> CreateAsync(CreateScheduledTransactionDto dto);
        Task<ScheduledTransactionDto> GetByIdAsync(Guid id);
        Task<PagedResult<ScheduledTransactionDto>> GetAllAsync(ScheduledTransactionFilterDto filter);
        Task DeactivateAsync(Guid id);

        Task<IEnumerable<ScheduledTransactionOccurrenceDto>> GetPendingOccurrencesAsync();

        Task<CreateTransactionResponse> ConfirmOccurrenceAsync(Guid occurrenceId, ConfirmScheduledTransactionDto? dto);

        Task SkipOccurrenceAsync(Guid occurrenceId);

        Task MarkAsSeenAsync(Guid occurrenceId);
    }
}
