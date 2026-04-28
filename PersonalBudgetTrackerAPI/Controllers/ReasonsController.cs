
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalBudgetTrackerAPI.DatabaseContext;
using Microsoft.AspNetCore.Authorization;
using PersonalBudgetTrackerAPI.Models.Entities;
using PersonalBudgetTrackerAPI.Services.Interfaces;
using PersonalBudgetTrackerAPI.Common;
using PersonalBudgetTrackerAPI.DTOs.Entities.ReasonDTOs;

namespace PersonalBudgetTrackerAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReasonsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IReasonService _reasonService;     

        public ReasonsController(ApplicationDbContext context, IReasonService reasonService)
        {
            _context = context;
            _reasonService = reasonService;
        }

        // GET: api/Reasons/details?page=2&pageSize=5
        [HttpGet("details")]
        public async Task<ActionResult<ApiResponse<PagedResult<ReasonDetailsDto>>>> GetReasonsWithDetails([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _reasonService.GetReasonsWithDetailsAsync(page, pageSize);
            return Ok(ApiResponse<PagedResult<ReasonDetailsDto>>.Ok(result, "Reasons retrieved successfully."));
        }

        // GET: api/Reasons?search=example&page=2&pageSize=5
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<ReasonDto>>>> GetReasons(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
        {
            var result = string.IsNullOrWhiteSpace(search)
                ? await _reasonService.GetUserReasonsAsync(page, pageSize)
                : await _reasonService.SearchReasonsAsync(search, page, pageSize);

            return Ok(ApiResponse<PagedResult<ReasonDto>>
                .Ok(result, "Reasons retrieved successfully."));
        }


    }
}
