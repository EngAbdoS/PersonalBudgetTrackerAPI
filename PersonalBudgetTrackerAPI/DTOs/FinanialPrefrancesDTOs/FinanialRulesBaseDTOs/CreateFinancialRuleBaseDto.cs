using PersonalBudgetTrackerAPI.Models.FinancialPrefrances;

namespace PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.FinanialRuleDTOs
{
    public abstract class CreateFinancialRuleBaseDto
    {
        public required string Title { get; set; }
        public string? Notes { get; set; }
        public Models.FinancialPrefrances.ValueType ValueType { get; set; }
        public decimal Value { get; set; }
        public ScopeType ScopeType { get; set; }
        public Guid? PaymentGatewayId { get; set; }
        public PeriodType PeriodType { get; set; }
        public DateTime? PeriodStart { get; set; }
        public DateTime? PeriodEnd { get; set; }
        public RecurrenceMode RecurrenceMode { get; set; }
        public PeriodType? RecurrencePeriod { get; set; }
    }
}
