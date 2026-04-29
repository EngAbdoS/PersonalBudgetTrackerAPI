using PersonalBudgetTrackerAPI.Models.Entities;

namespace PersonalBudgetTrackerAPI.DTOs.Entities.PaymentGatewayDtos
{
    public static class PaymentGatewayExtensions
    {
        public static PaymentGatewayDto ToDto(this PaymentGateway pg)
        {
            return new PaymentGatewayDto
            {
                Id = pg.Id,
                Title = pg.Title,
                BankName = pg.BankName,
                PaymentGatewayType = pg.PaymentGatewayType
            };
        }
    }
}   

