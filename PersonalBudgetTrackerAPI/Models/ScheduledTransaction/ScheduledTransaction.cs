using PersonalBudgetTrackerAPI.Models.Entities;
using PersonalBudgetTrackerAPI.Models.FinancialPrefrances;

namespace PersonalBudgetTrackerAPI.Models.ScheduledTransaction
{
    public class ScheduledTransaction : AuditableEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? TransactionDetails { get; set; }
        public decimal Amount { get; set; }
        public bool IsIncome { get; set; }
        public PaymentType PaymentType { get; set; }

        public Guid PaymentGatewayId { get; set; }
        public PaymentGateway PaymentGateway { get; set; } = null!;

        public Guid TransactionPartnerId { get; set; }
        public TransactionPartner TransactionPartner { get; set; } = null!;

        // Expense only
        public Guid? CategoryId { get; set; }
        public Category? Category { get; set; }
        public decimal? FeeAmount { get; set; }

        // Income only
        public Guid? ReasonId { get; set; }
        public Reason? Reason { get; set; }

        // Schedule-specific
        public bool IsFlexibleAmount { get; set; }
        public PeriodType PeriodType { get; set; }
        public DayOfWeek? DayOfWeek { get; set; }
        public int? DayOfMonth { get; set; }
        public DateTime NextDueDate { get; set; }
        public bool IsActive { get; set; }

        public ICollection<ScheduledTransactionOccurrence> Occurrences { get; set; } = [];
    }

}
