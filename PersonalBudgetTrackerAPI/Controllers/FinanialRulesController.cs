using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalBudgetTrackerAPI.Common;
using PersonalBudgetTrackerAPI.Common.Pagination;
using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.ExpenseLimitRuleDTOs;
using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.FinanialRuleDTOs;
using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.FinanialRulesBaseDTOs;
using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.MinimumBalanceRuleDTOs;
using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.SavingRuleDTOs;
using PersonalBudgetTrackerAPI.Services.Interfaces.FinanialPrefrances;

namespace PersonalBudgetTrackerAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FinancialRulesController : ControllerBase
    {
        private readonly IFinanialRuleService _service;

        public FinancialRulesController(IFinanialRuleService service)
        {
            _service = service;
        }


        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<FinancialRuleBaseDto>>>> Get(
            [FromQuery] FinancialRuleFilterDto filter)
        {
            var result = await _service.GetUserRulesAsync(filter);

            return Ok(ApiResponse<PagedResult<FinancialRuleBaseDto>>
                .Ok(result, "Financial rules retrieved successfully"));
        }

        // POST /financial-rules/expense-limits
        [HttpPost("expense-limits")]
        public async Task<IActionResult> CreateExpenseLimit(
            [FromBody] CreateExpenseLimitRuleDto dto)
        {
            var result = await _service.CreateExpenseLimitRuleAsync(dto);
            return Ok(ApiResponse<ExpenseLimitRuleDto>.Ok(result, "Expense limit rule created successfully"));
        }

        // POST /financial-rules/minimum-balances
        [HttpPost("minimum-balances")]
        public async Task<IActionResult> CreateMinimumBalance(
            [FromBody] CreateMinimumBalanceRuleDto dto)
        {
            var result = await _service.CreateMinimumBalanceRuleAsync(dto);
            return Ok(ApiResponse<MinimumBalanceRuleDto>.Ok(result, "Minimum balance rule created successfully"));
        }

        // POST /financial-rules/saving-goals
        [HttpPost("saving-goals")]
        public async Task<IActionResult> CreateSavingGoal(
            [FromBody] CreateSavingRuleDto dto)
        {
            var result = await _service.CreateSavingRuleAsync(dto);
            return Ok(ApiResponse<SavingRuleDto>.Ok(result, "Saving goal rule created successfully"));
        }

        // PUT /financial-rules/expense-limits/{id}
        [HttpPut("expense-limits/{id:guid}")]
        public async Task<IActionResult> UpdateExpenseLimit(
            [FromRoute] Guid id,
            [FromBody] UpdateExpenseLimitRuleDto dto)
        {
            var result = await _service.UpdateExpenseLimitRuleAsync(id, dto);
            return Ok(ApiResponse<ExpenseLimitRuleDto>.Ok(result, "Expense limit rule updated successfully"));
        }

        // PUT /financial-rules/minimum-balances/{id}
        [HttpPut("minimum-balances/{id:guid}")]
        public async Task<IActionResult> UpdateMinimumBalance(
            [FromRoute] Guid id,
            [FromBody] UpdateMinimumBalanceRuleDto dto)
        {
            var result = await _service.UpdateMinimumBalanceRuleAsync(id, dto);
            return Ok(ApiResponse<MinimumBalanceRuleDto>.Ok(result, "Minimum balance rule updated successfully"));
        }

        // PUT /financial-rules/saving-goals/{id}
        [HttpPut("saving-goals/{id:guid}")]
        public async Task<IActionResult> UpdateSavingGoal(
            [FromRoute] Guid id,
            [FromBody] UpdateSavingRuleDto dto)
        {
            var result = await _service.UpdateSavingRuleAsync(id, dto);
            return Ok(ApiResponse<SavingRuleDto>.Ok(result, "Saving goal rule updated successfully"));
        }

        [HttpGet("saving-goals-status")]
        public async Task<ActionResult<ApiResponse<List<SavingGoalStatusDto>>>> GetSavingGoalsStatus()
        {
            var result = await _service.GetSavingGoalsStatusAsync();

            return Ok(ApiResponse<List<SavingGoalStatusDto>>
                .Ok(result, "Saving goals status retrieved successfully"));
        }


        [HttpPatch("{id:guid}/activate")]
        public async Task<IActionResult> Activate([FromRoute] Guid id)
        {
            await _service.ActivateRuleAsync(id);

            return Ok(ApiResponse<string>
                .Ok("Activated", "Rule activated successfully"));
        }


        [HttpPatch("{id:guid}/deactivate")]
        public async Task<IActionResult> Deactivate([FromRoute] Guid id)
        {
            await _service.DeactivateRuleAsync(id);

            return Ok(ApiResponse<string>
                .Ok("Deactivated", "Rule deactivated successfully"));
        }
    }
}