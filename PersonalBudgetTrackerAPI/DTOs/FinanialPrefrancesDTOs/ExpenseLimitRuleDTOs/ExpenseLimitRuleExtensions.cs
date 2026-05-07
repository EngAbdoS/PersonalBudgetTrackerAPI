using PersonalBudgetTrackerAPI.Models.FinancialPrefrances;

namespace PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.ExpenseLimitRuleDTOs
{
    public static class ExpenseLimitRuleExtensions
    {
        public static ExpenseLimitRuleDto ToDto(this ExpenseLimitRule rule)
        {
            return new ExpenseLimitRuleDto
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
                CreatedAt = rule.CreatedAt,
                TargetType = rule.TargetType,
                CategoryId = rule.CategoryId,
                CategoryName = rule.Category?.Title,
                TransactionPartnerId = rule.TransactionPartnerId,
                TransactionPartnerName = rule.TransactionPartner?.Name
            };
        }

       
    }
}
