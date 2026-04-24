using System.ComponentModel.DataAnnotations;

namespace PersonalBudgetTrackerAPI.Models.Entities
{
    public class Expense : Transaction
    {
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public decimal FeeAmount { get; set; }

    }
}
