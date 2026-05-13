using PersonalBudgetTrackerAPI.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace PersonalBudgetTrackerAPI.Models.FinancialPrefrances
{
    public class FinancialRule : AuditableEntity
    {
        [Key] public Guid Id { get; set; } // what about cache the result ? or cash evaluation model for evaluating user rules 

        [Required] public required string Title { get; set; }
        public string? Notes { get; set; }

        // Value definition
        [Required] public ValueType ValueType { get; set; }   // Percentage | StaticAmount
        [Required] public decimal Value { get; set; }          // e.g. 10 = 10% of income, or 500 = $500

        // Scope: whole user or one gateway
        [Required] public ScopeType ScopeType { get; set; }
        public Guid? PaymentGatewayId { get; set; }
        public PaymentGateway? PaymentGateway { get; set; }

        // Period
        [Required] public PeriodType PeriodType { get; set; }
        public DateTime? PeriodStart { get; set; }             // required when PeriodType == Custom
        public DateTime? PeriodEnd { get; set; }
        public RecurrenceMode RecurrenceMode { get; set; }

        // Only relevant when RecurrenceMode = Recurring
        // Tells the system HOW to roll the period forward
        public PeriodType? RecurrencePeriod { get; set; }
        // e.g. rule has Custom dates but recurs Monthly

        public DateTime? LastActivatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }

        public bool IsActive { get; set; } = true; // over write the query filter

    }/*
 rules validating ber period , active , value within total spend in this period 
 
in creaing new rule must chek if there is no other active rule with same scope and target to avoid conflict between rules
 */
}
