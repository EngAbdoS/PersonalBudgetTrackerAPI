using PersonalBudgetTrackerAPI.Models.FinancialPrefrances;

namespace PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.SavingRuleDTOs
{
    public static class SavingRuleExtensions
    {
        public static SavingRuleDto ToDto(this SavingRule rule)
        {
            return new SavingRuleDto
            {
                Id = rule.Id,
                Title = rule.Title,
                Notes = rule.Notes,
                ValueType = rule.ValueType,
                Value = rule.Value,
                ScopeType = rule.ScopeType,
                PaymentGatewayId = rule.PaymentGatewayId,
                PaymentGatewayName = rule.PaymentGateway?.Title,
                PeriodType = rule.PeriodType,
                PeriodStart = rule.PeriodStart,
                PeriodEnd = rule.PeriodEnd,
                RecurrenceMode = rule.RecurrenceMode,
                RecurrencePeriod = rule.RecurrencePeriod,
                IsActive = rule.IsActive,
                ExpiresAt = rule.ExpiresAt,
                CreatedAt = rule.CreatedAt
            };
        }

     
    }
}
