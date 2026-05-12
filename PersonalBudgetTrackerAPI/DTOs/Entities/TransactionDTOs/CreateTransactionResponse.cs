using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.FinanialRulesBaseDTOs;
using PersonalBudgetTrackerAPI.Models.FinancialPrefrances;

namespace PersonalBudgetTrackerAPI.DTOs.Entities.TransactionDTOs
{
    public class CreateTransactionResponse
    {
        public TransactionDto? Transaction { get; set; }

        public Guid? CachedTransactionId { get; set; }

        public TransactionValidationResult? RuleValidation { get; set; }
    }

    public class TransactionValidationResult
    {
        public bool IsValid => !Violations.Any(v => v.Status == RuleStatus.Violated);
        public List<RuleViolationDto> Violations { get; set; } = [];
    }
}
