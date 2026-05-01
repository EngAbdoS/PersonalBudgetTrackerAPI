using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalBudgetTrackerAPI.Common;
using PersonalBudgetTrackerAPI.DTOs.Entities.TransactionDTOs;
using PersonalBudgetTrackerAPI.Services.Interfaces;

namespace PersonalBudgetTrackerAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _service;
        public TransactionsController(ITransactionService service)
        {
            _service = service;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponse<TransactionDto>>> GetById([FromRoute]Guid id)
        {
            var result = await _service.GetByIdAsync(id);

            return Ok(ApiResponse<TransactionDto>
                .Ok(result, "Transaction retrieved successfully"));
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<TransactionDto>>>> Get([FromQuery] TransactionFilterDto filter)
        {
            var result = await _service.GetUserTransactionsAsync(filter);

            return Ok(ApiResponse<PagedResult<TransactionDto>>
                .Ok(result, "Transactions retrieved successfully"));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<TransactionDto>>> Create([FromBody] CreateTransactionDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return Ok(ApiResponse<TransactionDto>.Ok(result, "Transaction created"));
        }

        // GET: api/transactions/requirements
        [HttpGet("requirements")]
        public async Task<ActionResult<ApiResponse<TransactionRequirementsDto>>> GetRequirements()
        {
            var result = await _service.GetRequirementsAsync();

            return Ok(ApiResponse<TransactionRequirementsDto>
                .Ok(result, "Transaction requirements retrieved successfully"));
        }


    }
}
