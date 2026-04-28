namespace PersonalBudgetTrackerAPI.DTOs.Entities.ReasonDTOs
{
    public class ReasonDto
    {
        public  Guid ReasonId { get; set; }

        public string ReasonDetails { get; set; } = string.Empty;

    }
}
