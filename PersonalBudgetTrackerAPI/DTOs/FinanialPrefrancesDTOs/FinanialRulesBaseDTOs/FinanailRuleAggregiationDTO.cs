using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.ExpenseLimitRuleDTOs;
using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.FinanialRuleDTOs;
using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.MinimumBalanceRuleDTOs;
using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.SavingRuleDTOs;
using PersonalBudgetTrackerAPI.Models.FinancialPrefrances;

namespace PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.FinanialRulesBaseDTOs
{
    public class RuleAggregiationInputDTO
    {
        public ScopeType ScopeType { get; set; }
        public Guid? PaymentGatewayId { get; set; }


        public LimitTargetType? TargetType { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? TransactionPartnerId { get; set; }


        public DateOnly From { get; set; }
        public DateOnly To { get; set; }
    }


    public static class RuleAggregationMapper
    {
        public static RuleAggregiationInputDTO ToAggregationInput(
            this FinancialRuleBaseDto rule)
        {
            var dto = new RuleAggregiationInputDTO
            {
                ScopeType = rule.ScopeType,
                PaymentGatewayId = rule.PaymentGatewayId,
                From = DateOnly.FromDateTime(rule.PeriodStart??DateTime.UtcNow),
                To = DateOnly.FromDateTime(rule.PeriodEnd??DateTime.UtcNow)
            };

            switch (rule)
            {
                case ExpenseLimitRuleDto expenseRule:
                    dto.TargetType = expenseRule.TargetType;
                    dto.CategoryId = expenseRule.CategoryId;
                    dto.TransactionPartnerId = expenseRule.TransactionPartnerId;
                    break;

                case MinimumBalanceRuleDto:
                    dto.TargetType = null;
                    break;
                case SavingRuleDto:
                    dto.TargetType = null;

                    break;

                default:
                    throw new NotSupportedException(
                        $"Unsupported rule type: {rule.GetType().Name}");
            }

            return dto;
        }
    }
}
