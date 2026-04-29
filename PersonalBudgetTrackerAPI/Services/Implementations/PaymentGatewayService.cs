using PersonalBudgetTrackerAPI.DTOs.Entities.PaymentGatewayDtos;
using PersonalBudgetTrackerAPI.Services.Interfaces;

namespace PersonalBudgetTrackerAPI.Services.Implementations
{
    public class PaymentGatewayService : IPaymentGatewayService
    {
        public Task<PaymentGatewayDto> CreateAsync(CreatePaymentGatewayDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<PaymentGatewayDetailsDto> GetDetailsByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<PaymentGatewayDto>> GetUserPaymentGatewaysAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> PaymentGatewayValidAndExist(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
