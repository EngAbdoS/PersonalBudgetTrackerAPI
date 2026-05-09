using PersonalBudgetTrackerAPI.Common.Pagination;
using PersonalBudgetTrackerAPI.Models.FinancialPrefrances;

namespace PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.FinanialRulesBaseDTOs
{
    public class FinancialRuleSearchFilterDto : PaginationQuery
    {
        public string? Search { get; set; }

        public FinancialRuleType? RuleType { get; set; }

        public ScopeType? ScopeType { get; set; }

        public PeriodType? PeriodType { get; set; }

        public RecurrenceMode? RecurrenceMode { get; set; }

        public Models.FinancialPrefrances.ValueType? ValueType { get; set; }

        public bool? IsActive { get; set; }

        public Guid? PaymentGatewayId { get; set; }

        public decimal? MinValue { get; set; }

        public decimal? MaxValue { get; set; }

        public DateTime? StartDateFrom { get; set; }

        public DateTime? StartDateTo { get; set; }

        public DateTime? EndDateFrom { get; set; }

        public DateTime? EndDateTo { get; set; }

    }
}
