using PersonalBudgetTrackerAPI.Models.FinancialPrefrances;

namespace PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.MinimumBalanceRuleDTOs
{
    public static class MinimumBalanceRuleExtensions
    {
        public static MinimumBalanceRuleDto ToDto(this MinimumBalanceRule rule)
        {
            return new MinimumBalanceRuleDto
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
