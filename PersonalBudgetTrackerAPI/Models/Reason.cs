using System.ComponentModel.DataAnnotations;

namespace PersonalBudgetTrackerAPI.Models
{
    public class Reason
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        public required string ReasonDetails { get; set; }


    }
}
