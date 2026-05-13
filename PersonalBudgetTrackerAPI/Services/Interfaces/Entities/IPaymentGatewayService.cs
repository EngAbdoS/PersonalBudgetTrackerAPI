using PersonalBudgetTrackerAPI.DTOs.Entities.PaymentGatewayDtos;

namespace PersonalBudgetTrackerAPI.Services.Interfaces.Entities
{
    public interface IPaymentGatewayService
    {
        Task<PaymentGatewayDto> CreateAsync(CreatePaymentGatewayDto dto);

        Task<bool> PaymentGatewayValidAndExist(Guid id);

        Task<List<PaymentGatewayDto>> GetUserPaymentGatewaysAsync();

        Task<PaymentGatewayDetailsDto> GetDetailsByIdAsync(Guid id);

        Task<List<PartnerPaymentGatewayStatsDto>> GetPaymentGatewayStats(Guid id);
    }
}
