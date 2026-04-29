namespace PersonalBudgetTrackerAPI.DTOs.Entities.CategoryDTOs
{
    public class CreateCategoryDto
    {
        public string Title { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public bool IsNeedful { get; set; }
        public decimal NeedPriority { get; set; }
    }
}
