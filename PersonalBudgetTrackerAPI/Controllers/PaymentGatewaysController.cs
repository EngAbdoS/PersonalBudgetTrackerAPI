using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalBudgetTrackerAPI.Common;
using PersonalBudgetTrackerAPI.DTOs.Entities.PaymentGatewayDtos;
using PersonalBudgetTrackerAPI.Services.Interfaces;

namespace PersonalBudgetTrackerAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentGatewaysController : ControllerBase
    {
        private readonly IPaymentGatewayService _service;


        public PaymentGatewaysController(IPaymentGatewayService service)
        {
            _service = service;
        }

        // POST: api/PaymentGateways
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePaymentGatewayDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return Ok(ApiResponse<PaymentGatewayDto>.Ok(result, "Payment gateway created"));
        }

        // GET: api/PaymentGateways
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetUserPaymentGatewaysAsync();
            return Ok(ApiResponse<List<PaymentGatewayDto>>.Ok(result, "Payment gateways retrieved"));
        }

        // GET: api/PaymentGateways/5
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetDetails([FromRoute] Guid id)
        {
            var result = await _service.GetDetailsByIdAsync(id);
            return Ok(ApiResponse<PaymentGatewayDetailsDto>.Ok(result, "Payment gateway details retrieved"));
        }


    }
}
