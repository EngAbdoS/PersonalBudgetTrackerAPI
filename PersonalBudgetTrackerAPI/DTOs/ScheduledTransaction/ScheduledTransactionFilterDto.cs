using PersonalBudgetTrackerAPI.Models.FinancialPrefrances;

namespace PersonalBudgetTrackerAPI.DTOs.ScheduledTransaction
{
    public class ScheduledTransactionFilterDto
    {
        public string? Search { get; set; }
        public bool? IsIncome { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsFlexibleAmount { get; set; }
        public PeriodType? PeriodType { get; set; }
        public Guid? PaymentGatewayId { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? ReasonId { get; set; }

        public DateTime? DueDateFrom { get; set; }
        public DateTime? DueDateTo { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
    }
}
