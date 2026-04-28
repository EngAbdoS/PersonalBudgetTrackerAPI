namespace PersonalBudgetTrackerAPI.DTOs.Entities.ReasonDTOs
{
    public class ReasonDetailsDto
    {
        public Guid ReasonId { get; set; }
        public string ReasonDetails { get; set; } = string.Empty;
        public int UsageCount { get; set; }
        public decimal TotalIncome { get; set; }
    }
}
