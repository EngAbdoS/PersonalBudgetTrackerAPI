namespace PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.SavingRuleDTOs
{
    public class SavingGoalStatusDto
    {
        public Guid RuleId { get; set; }

        public string Title { get; set; } = string.Empty;

        public decimal TargetAmount { get; set; }

        public decimal CurrentSavedAmount { get; set; }

        public decimal RemainingAmount { get; set; }

        public decimal ProgressPercentage { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime? TargetDate { get; set; }
    }
}
