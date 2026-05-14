using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalBudgetTrackerAPI.Common;
using PersonalBudgetTrackerAPI.Common.Pagination;
using PersonalBudgetTrackerAPI.DTOs.Entities.ScheduledTransactionDTOs;
using PersonalBudgetTrackerAPI.DTOs.Entities.TransactionDTOs;
using PersonalBudgetTrackerAPI.DTOs.ScheduledTransaction;
using PersonalBudgetTrackerAPI.Services.Interfaces.Entities;

namespace PersonalBudgetTrackerAPI.Controllers
{
    [Authorize]
    [Route("api/scheduled-transactions")]
    [ApiController]
    public class ScheduledTransactionsController : ControllerBase
    {
        private readonly IScheduledTransactionService _service;

        public ScheduledTransactionsController(IScheduledTransactionService service)
        {
            _service = service;
        }


        [HttpPost]
        public async Task<ActionResult<ApiResponse<ScheduledTransactionDto>>> Create(
            [FromBody] CreateScheduledTransactionDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return Ok(ApiResponse<ScheduledTransactionDto>.Ok(result, "Scheduled transaction created successfully."));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponse<ScheduledTransactionDto>>> GetById([FromRoute] Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(ApiResponse<ScheduledTransactionDto>.Ok(result, "Scheduled transaction retrieved successfully."));
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<ScheduledTransactionDto>>>> GetAll(
            [FromQuery] ScheduledTransactionFilterDto filter)
        {
            var result = await _service.GetAllAsync(filter);
            return Ok(ApiResponse<PagedResult<ScheduledTransactionDto>>.Ok(result, "Scheduled transactions retrieved successfully."));
        }

        [HttpPatch("{id:guid}/deactivate")]
        public async Task<ActionResult<ApiResponse<string?>>> Deactivate([FromRoute] Guid id)
        {
            await _service.DeactivateAsync(id);
            return Ok(ApiResponse<string?>.Ok(null, "Scheduled transaction deactivated."));
        }


        [HttpGet("occurrences/pending")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ScheduledTransactionOccurrenceDto>>>> GetPendingOccurrences()
        {
            var result = await _service.GetPendingOccurrencesAsync();
            return Ok(ApiResponse<IEnumerable<ScheduledTransactionOccurrenceDto>>.Ok(result, "Pending occurrences retrieved successfully."));
        }


        [HttpPost("occurrences/{occurrenceId:guid}/confirm")]
        public async Task<ActionResult<ApiResponse<CreateTransactionResponse>>> ConfirmOccurrence(
            [FromRoute] Guid occurrenceId,
            [FromBody] ConfirmScheduledTransactionDto? dto = null)
        {
            var result = await _service.ConfirmOccurrenceAsync(occurrenceId, dto);
            return Ok(ApiResponse<CreateTransactionResponse>.Ok(result, "Occurrence confirmed and transaction created."));
        }

        [HttpPost("occurrences/{occurrenceId:guid}/skip")]
        public async Task<ActionResult<ApiResponse<string?>>> SkipOccurrence([FromRoute] Guid occurrenceId)
        {
            await _service.SkipOccurrenceAsync(occurrenceId);
            return Ok(ApiResponse<string?>.Ok(null, "Occurrence skipped."));
        }

        [HttpPost("occurrences/{occurrenceId:guid}/seen")]
        public async Task<ActionResult<ApiResponse<string?>>> MarkAsSeen([FromRoute] Guid occurrenceId)
        {
            await _service.MarkAsSeenAsync(occurrenceId);
            return Ok(ApiResponse<string?>.Ok(null, "Occurrence marked as seen."));
        }
    }
}
