using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.FinanialRuleDTOs;
using PersonalBudgetTrackerAPI.Models.FinancialPrefrances;

namespace PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.ExpenseLimitRuleDTOs
{
    public class ExpenseLimitRuleDto :FinancialRuleBaseDto
    {
        public LimitTargetType TargetType { get; set; }
        public Guid? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public Guid? TransactionPartnerId { get; set; }
        public string? TransactionPartnerName { get; set; }
    }
}
