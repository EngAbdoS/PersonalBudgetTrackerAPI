using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalBudgetTrackerAPI.Common;
using PersonalBudgetTrackerAPI.Common.Pagination;
using PersonalBudgetTrackerAPI.DTOs.Entities.TransactionDTOs;
using PersonalBudgetTrackerAPI.Services.Implementations;
using PersonalBudgetTrackerAPI.Services.Interfaces.Entities;

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
        public async Task<ActionResult<ApiResponse<CreateTransactionResponse>>> Create([FromBody] CreateTransactionDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return Ok(ApiResponse<CreateTransactionResponse>.Ok(result, "Transaction created"));
        }

        [HttpPost("confirm/{cachedId}")]
        public async Task<ActionResult<ApiResponse<CreateTransactionResponse>>> ConfirmPendingTransaction([FromRoute]Guid cachedId)
        {
            var result = await _service.ConfirmPendingTransactionAsync(cachedId);
            return Ok(ApiResponse<CreateTransactionResponse>.Ok(result, "Pending transaction confirmed"));
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
