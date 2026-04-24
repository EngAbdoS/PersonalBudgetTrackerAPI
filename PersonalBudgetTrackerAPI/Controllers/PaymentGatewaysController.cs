using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalBudgetTrackerAPI.DatabaseContext;

using Microsoft.AspNetCore.Authorization;
using PersonalBudgetTrackerAPI.Models.Entities;

namespace PersonalBudgetTrackerAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentGatewaysController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PaymentGatewaysController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/PaymentGateways
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentGateway>>> GetPaymentGateway()
        {
            return await _context.PaymentGateway.ToListAsync();
        }

        // GET: api/PaymentGateways/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentGateway>> GetPaymentGateway(Guid id)
        {
            var paymentGateway = await _context.PaymentGateway.FindAsync(id);

            if (paymentGateway == null)
            {
                return NotFound();
            }

            return paymentGateway;
        }

        // PUT: api/PaymentGateways/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPaymentGateway(Guid id, PaymentGateway paymentGateway)
        {
            if (id != paymentGateway.Id)
            {
                return BadRequest();
            }

            _context.Entry(paymentGateway).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentGatewayExists(id))
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

        // POST: api/PaymentGateways
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<PaymentGateway>> PostPaymentGateway(PaymentGateway paymentGateway)
        {
            _context.PaymentGateway.Add(paymentGateway);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPaymentGateway", new { id = paymentGateway.Id }, paymentGateway);
        }

        // DELETE: api/PaymentGateways/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaymentGateway(Guid id)
        {
            var paymentGateway = await _context.PaymentGateway.FindAsync(id);
            if (paymentGateway == null)
            {
                return NotFound();
            }

            _context.PaymentGateway.Remove(paymentGateway);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PaymentGatewayExists(Guid id)
        {
            return _context.PaymentGateway.Any(e => e.Id == id);
        }
    }
}
