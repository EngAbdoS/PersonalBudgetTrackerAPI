
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

        // GET: api/Reasons?page=2&pageSize=5
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<ReasonDto>>>> GetReason([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _reasonService.GetUserReasonsAsync(page, pageSize);
            return Ok(ApiResponse<PagedResult<ReasonDto>>.Ok(result, "Reasons retrieved successfully."));
        }









        // GET: api/Reasons/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Reason>> GetReason(Guid id)
        {
            var reason = await _context.Reason.FindAsync(id);

            if (reason == null)
            {
                return NotFound();
            }

            return reason;
        }

        // PUT: api/Reasons/5
        //[Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReason([FromRoute] Guid id, [FromBody] Reason reason)
        {
            if (id != reason.Id)
            {
                return BadRequest();
            }

            _context.Entry(reason).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReasonExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Reasons
        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Reason>> PostReason([FromBody]  String reasonDetails) // will be removed , adding reason in transaction creation
        {
           var reason = await _reasonService.CreateReasonAsync(reasonDetails);

            return CreatedAtAction("GetReason", new { id = reason.ReasonId }, reason);
        }

        // DELETE: api/Reasons/5
        ///[Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReason(Guid id)
        {
            var reason = await _context.Reason.FindAsync(id);
            if (reason == null)
            {
                return NotFound();
            }

            _context.Reason.Remove(reason);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ReasonExists(Guid id)
        {
            return _context.Reason.Any(e => e.Id == id);
        }
    }
}
