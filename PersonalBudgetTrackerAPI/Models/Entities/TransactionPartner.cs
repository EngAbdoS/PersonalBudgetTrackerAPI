using System.ComponentModel.DataAnnotations;

namespace PersonalBudgetTrackerAPI.Models.Entities
{
    public class TransactionPartner : AuditableEntity
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public required string Info { get; set; }

        [Required]
        public required string Location { get; set; }

        [Required]
        public required string Contact { get; set; }

    }
}
