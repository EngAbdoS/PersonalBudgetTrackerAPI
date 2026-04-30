using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalBudgetTrackerAPI.Common;
using PersonalBudgetTrackerAPI.DTOs.Entities.TransactionPartnerDTOs;
using PersonalBudgetTrackerAPI.Services.Interfaces;


namespace PersonalBudgetTrackerAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionPartnersController : ControllerBase
    {
        private readonly ITransactionPartnerService _service;
        public TransactionPartnersController(ITransactionPartnerService service)
        {
            _service = service;
        }

        // GET: api/TransactionPartners
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1,[FromQuery] int pageSize = 10)
        {
            var result = await _service.GetAllAsync(page, pageSize);

            return Ok(ApiResponse<PagedResult<TransactionPartnerDto>>
                .Ok(result, "Transaction partners retrieved"));
        }

        // POST: api/TransactionPartners
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTransactionPartnerDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return Ok(ApiResponse<TransactionPartnerDto>.Ok(result, "Created successfully"));
        }


        // PUT: api/TransactionPartners/5
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateTransactionPartnerDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return Ok(ApiResponse<TransactionPartnerDto>.Ok(result, "Updated successfully"));
        }

        // DELETE: api/TransactionPartners/5
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            await _service.DeleteAsync(id);
            return Ok(ApiResponse<string>.Ok("Deleted", "Deleted successfully"));
        }

        // GET: api/TransactionPartners/5
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetDetails([FromRoute] Guid id)
        {
            var result = await _service.GetDetailsAsync(id);
            return Ok(ApiResponse<TransactionPartnerDetailsDto>.Ok(result, "Transaction partner details retrieved"));
        }

    
    }
}
