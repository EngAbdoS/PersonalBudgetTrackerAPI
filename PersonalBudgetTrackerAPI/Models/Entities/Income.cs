using System.ComponentModel.DataAnnotations;

namespace PersonalBudgetTrackerAPI.Models.Entities
{
    public class Income :Transaction
    {

        public Guid ReasonId { get; set; }
        public Reason Reason { get; set; } = null!;

    }
}
