using PersonalBudgetTrackerAPI.DTOs.Entities.TransactionDTOs;

namespace PersonalBudgetTrackerAPI.Services.Interfaces.FinanialPrefrances
{
    public interface ITransactionValidationService
    {
        Task<TransactionValidationResult> ValidateExpenseAsync(ExpenseTransactionValidationDto dto);

    }
}
