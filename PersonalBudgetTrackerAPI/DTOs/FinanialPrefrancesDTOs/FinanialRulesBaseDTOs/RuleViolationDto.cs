using PersonalBudgetTrackerAPI.Models.FinancialPrefrances;

namespace PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.FinanialRulesBaseDTOs
{
    public class RuleViolationDto
    {
        public Guid RuleId { get; set; }
        public string RuleType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public decimal Threshold { get; set; }
        public decimal CurrentTotal { get; set; }
        public decimal AfterTransaction { get; set; }
        public decimal Excess => AfterTransaction - Threshold;
        public RuleStatus Status { get; set; }
    }
}
