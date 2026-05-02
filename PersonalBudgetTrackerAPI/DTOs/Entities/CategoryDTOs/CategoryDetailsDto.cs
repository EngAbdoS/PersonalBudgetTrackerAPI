using PersonalBudgetTrackerAPI.DTOs.Entities.TransactionPartnerDTOs;

namespace PersonalBudgetTrackerAPI.DTOs.Entities.CategoryDTOs
{
    public class CategoryDetailsDto
    {
        public Guid CategoryId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public bool IsNeedful { get; set; }
        public decimal NeedPriority { get; set; }
        public int UsageCount { get; set; }
        public decimal TotalExpense { get; set; }

        public List<TransactionPartnerDto> TransactionPartners { get; set; } = new();
    }
}
