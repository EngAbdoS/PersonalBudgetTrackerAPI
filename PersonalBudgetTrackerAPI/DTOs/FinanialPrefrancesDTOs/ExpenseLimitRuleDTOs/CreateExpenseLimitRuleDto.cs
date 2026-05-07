using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.FinanialRuleDTOs;
using PersonalBudgetTrackerAPI.Models.FinancialPrefrances;

namespace PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.ExpenseLimitRuleDTOs
{
    public class CreateExpenseLimitRuleDto: CreateFinancialRuleBaseDto
    {
        public LimitTargetType TargetType { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? TransactionPartnerId { get; set; }
    }
}
