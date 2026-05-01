
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalBudgetTrackerAPI.Common;
using PersonalBudgetTrackerAPI.Common.Pagination;
using PersonalBudgetTrackerAPI.DatabaseContext;
using PersonalBudgetTrackerAPI.DTOs.Entities.ReasonDTOs;
using PersonalBudgetTrackerAPI.DTOs.Entities.TransactionDTOs;
using PersonalBudgetTrackerAPI.Services.Interfaces;

namespace PersonalBudgetTrackerAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReasonsController : ControllerBase
    {
        private readonly IReasonService _reasonService;     
        private readonly ITransactionService _transactionService;

        public ReasonsController(ApplicationDbContext context, IReasonService reasonService, ITransactionService transactionService)
        {
            _reasonService = reasonService;
            _transactionService = transactionService;
        }

        // GET: api/Reasons/details?page=2&pageSize=5
        [HttpGet("details")]
        public async Task<ActionResult<ApiResponse<PagedResult<ReasonDetailsDto>>>> GetReasonsWithDetails([FromQuery] PaginationQuery query)
        {
            var result = await _reasonService.GetReasonsWithDetailsAsync(query);
            return Ok(ApiResponse<PagedResult<ReasonDetailsDto>>.Ok(result, "Reasons retrieved successfully."));
        }

        // GET: api/Reasons?search=example&page=2&pageSize=5
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<ReasonDto>>>> GetReasons(
        [FromQuery] string? search,
        [FromQuery] PaginationQuery query)
        {
            var result = string.IsNullOrWhiteSpace(search)
                ? await _reasonService.GetUserReasonsAsync(query)
                : await _reasonService.SearchReasonsAsync(search, query);

            return Ok(ApiResponse<PagedResult<ReasonDto>>
                .Ok(result, "Reasons retrieved successfully."));
        }


        [HttpGet("{id:guid}/transactions")]
        public async Task<ActionResult<ApiResponse<List<TransactionSimpleDto>>>> GetTransactions([FromRoute] Guid id)
        {
            var result = await _transactionService.GetByReasonIdAsync(id);

            return Ok(ApiResponse<List<TransactionSimpleDto>>
                .Ok(result, "Transactions retrieved successfully"));
        }

    }
}
