using System.ComponentModel.DataAnnotations;

namespace PersonalBudgetTrackerAPI.Models
{
    public class Category
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        public required string Title { get; set; }
        [Required]
        public required string Details { get; set; }
        [Required]
        public bool IsNeedful { get; set; }

        [Required]
        public decimal NeedPriority { get; set; }

    }
}
