using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalBudgetTrackerAPI.DatabaseContext;
using PersonalBudgetTrackerAPI.Models;

namespace PersonalBudgetTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionPartnersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TransactionPartnersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/TransactionPartners
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionPartner>>> GetTransactionPartner()
        {
            return await _context.TransactionPartner.ToListAsync();
        }

        // GET: api/TransactionPartners/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionPartner>> GetTransactionPartner(Guid id)
        {
            var transactionPartner = await _context.TransactionPartner.FindAsync(id);

            if (transactionPartner == null)
            {
                return NotFound();
            }

            return transactionPartner;
        }

        // PUT: api/TransactionPartners/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransactionPartner(Guid id, TransactionPartner transactionPartner)
        {
            if (id != transactionPartner.Id)
            {
                return BadRequest();
            }

            _context.Entry(transactionPartner).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionPartnerExists(id))
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

        // POST: api/TransactionPartners
        [HttpPost]
        public async Task<ActionResult<TransactionPartner>> PostTransactionPartner(TransactionPartner transactionPartner)
        {
            _context.TransactionPartner.Add(transactionPartner);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTransactionPartner", new { id = transactionPartner.Id }, transactionPartner);
        }

        // DELETE: api/TransactionPartners/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransactionPartner(Guid id)
        {
            var transactionPartner = await _context.TransactionPartner.FindAsync(id);
            if (transactionPartner == null)
            {
                return NotFound();
            }

            _context.TransactionPartner.Remove(transactionPartner);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TransactionPartnerExists(Guid id)
        {
            return _context.TransactionPartner.Any(e => e.Id == id);
        }
    }
}
