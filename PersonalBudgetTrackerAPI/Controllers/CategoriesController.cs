using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonalBudgetTrackerAPI.Common;
using PersonalBudgetTrackerAPI.DatabaseContext;
using PersonalBudgetTrackerAPI.DTOs.Entities.CategoryDTOs;
using PersonalBudgetTrackerAPI.Models.Entities;
using PersonalBudgetTrackerAPI.Services.Interfaces;

namespace PersonalBudgetTrackerAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;
        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }


        // POST: api/Categories
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            var result = await _service.CreateCategoryAsync(dto);
            return Ok(ApiResponse<CategoryDto>.Ok(result,"Category created successfully"));
        }


        // PUT: api/Categories/5
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateCategoryDto dto)
        {
            var result = await _service.UpdateCategoryAsync(id, dto);
            return Ok(ApiResponse<CategoryDto>.Ok(result,"Category updated successfully"));
        }


        // DELETE: api/Categories/5
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            await _service.DeleteCategoryAsync(id);
            return Ok(ApiResponse<string>.Ok("Deleted successfully","Category deleted successfully"));
        }


        // GET: api/Categories
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _service.GetCategoryByIdAsync(id);
            return Ok(ApiResponse<CategoryDto>.Ok(result,"Category retrieved successfully"));
        }


        // GET: api/Categories/details?page=2&pageSize=5
        [HttpGet("details")]
        public async Task<IActionResult> GetDetails([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetCategoriesWithDetailsAsync(page, pageSize);
            return Ok(ApiResponse<PagedResult<CategoryDetailsDto>>.Ok(result,"Categories retrieved successfully"));
        }


        // GET: api/Categories?search=example&isNeedful=true&minPriority=1&maxPriority=5&page=2&pageSize=5
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] string? search,
            [FromQuery] bool? isNeedful,
            [FromQuery] decimal? minPriority,
            [FromQuery] decimal? maxPriority,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _service.SearchCategoriesAsync(
                search, isNeedful, minPriority, maxPriority, page, pageSize);

            return Ok(ApiResponse<PagedResult<CategoryDto>>.Ok(result,"Categories retrieved successfully"));
        }


    }
}
