using System.ComponentModel.DataAnnotations;

namespace PersonalBudgetTrackerAPI.Models.Entities
{
    public class Reason : AuditableEntity
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        public required string ReasonDetails { get; set; }


    }
}
