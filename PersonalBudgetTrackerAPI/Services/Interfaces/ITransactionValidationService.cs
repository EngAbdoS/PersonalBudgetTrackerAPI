using PersonalBudgetTrackerAPI.DTOs.Entities.TransactionDTOs;

namespace PersonalBudgetTrackerAPI.Services.Interfaces
{
    public interface ITransactionValidationService
    {
        Task<TransactionValidationResult> ValidateExpenseAsync(ExpenseTransactionValidationDto dto);

    }
}
