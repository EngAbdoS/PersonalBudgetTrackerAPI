using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalBudgetTrackerAPI.DatabaseContext;
using PersonalBudgetTrackerAPI.Models;

using Microsoft.AspNetCore.Authorization;

namespace PersonalBudgetTrackerAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReasonsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReasonsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Reasons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reason>>> GetReason()
        {
            return await _context.Reason.ToListAsync();
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
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReason(Guid id, Reason reason)
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
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Reason>> PostReason(Reason reason)
        {
            _context.Reason.Add(reason);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReason", new { id = reason.Id }, reason);
        }

        // DELETE: api/Reasons/5
        [Authorize(Roles = "Admin")]
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
