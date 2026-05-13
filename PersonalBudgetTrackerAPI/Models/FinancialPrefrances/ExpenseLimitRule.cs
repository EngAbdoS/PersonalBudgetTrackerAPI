using PersonalBudgetTrackerAPI.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace PersonalBudgetTrackerAPI.Models.FinancialPrefrances
{
    public class ExpenseLimitRule : FinancialRule
    {
      

        // What this limit applies to (Category XOR TransactionPartner or All ) => when all the rule is expense limit for gateway or user in general
        [Required] 
        public LimitTargetType TargetType { get; set; }
        public Guid? CategoryId { get; set; }
        public Category? Category { get; set; }
        public Guid? TransactionPartnerId { get; set; }
        public TransactionPartner? TransactionPartner { get; set; }

    }
}
