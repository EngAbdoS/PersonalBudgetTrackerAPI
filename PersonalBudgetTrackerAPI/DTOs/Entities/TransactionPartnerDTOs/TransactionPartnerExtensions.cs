using PersonalBudgetTrackerAPI.Models.Entities;

namespace PersonalBudgetTrackerAPI.DTOs.Entities.TransactionPartnerDTOs
{
    public static class TransactionPartnerExtensions
    {
        public static TransactionPartnerDto ToDto(this TransactionPartner entity)
        {
            return new TransactionPartnerDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Location = entity.Location,
                Info = entity.Info,
                Contact = entity.Contact
            };
        }
    }
}
