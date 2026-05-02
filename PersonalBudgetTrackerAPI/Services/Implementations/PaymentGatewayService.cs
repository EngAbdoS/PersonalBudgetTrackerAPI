using Microsoft.EntityFrameworkCore;
using PersonalBudgetTrackerAPI.Common.Exceptions;
using PersonalBudgetTrackerAPI.DatabaseContext;
using PersonalBudgetTrackerAPI.DTOs.Entities.PaymentGatewayDtos;
using PersonalBudgetTrackerAPI.Models.Entities;
using PersonalBudgetTrackerAPI.Services.Interfaces;

namespace PersonalBudgetTrackerAPI.Services.Implementations
{
    public class PaymentGatewayService : IPaymentGatewayService
    {
        private readonly ApplicationDbContext _context;

        public PaymentGatewayService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> PaymentGatewayValidAndExist(Guid id)
        {
            return await _context.PaymentGateway
                .AnyAsync(p => p.Id == id && !p.IsDeleted && p.ExpirationDate > DateTime.UtcNow);
        }
        public async Task<PaymentGatewayDto> CreateAsync(CreatePaymentGatewayDto dto)
        {
            var gateway = new PaymentGateway
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                BankName = dto.BankName,
                PaymentGatewayType = dto.PaymentGatewayType,
                InitialBalance = dto.InitialBalance,
                ExpirationDate = dto.ExpirationDate
            };

            _context.PaymentGateway.Add(gateway);
            await _context.SaveChangesAsync();

            return gateway.ToDto();
        }
        public async Task<List<PaymentGatewayDto>> GetUserPaymentGatewaysAsync()
        {
            var gateways = await _context.PaymentGateway
                .OrderByDescending(p => p.InitialBalance)
                .ToListAsync();

            return gateways.Select(g => g.ToDto()).ToList() ?? [];
        }

        public async Task<PaymentGatewayDetailsDto> GetDetailsByIdAsync(Guid id)
        {
            var gateway = await _context.PaymentGateway
                .FirstOrDefaultAsync(p => p.Id == id)
                ?? throw new NotFoundException("Payment gateway not found");

            var income = await _context.Set<Income>()
                .Where(i => i.PaymentGatewayId == id)
                .SumAsync(i => (decimal?)i.Amount) ?? 0;

            var expense = await _context.Set<Expense>()
                .Where(e => e.PaymentGatewayId == id)
                .SumAsync(e => (decimal?)e.Amount) ?? 0;

            var incomeUsage = await _context.Set<Income>()
                .CountAsync(i => i.PaymentGatewayId == id);

            var expenseUsage = await _context.Set<Expense>()
                .CountAsync(e => e.PaymentGatewayId == id);


            return new PaymentGatewayDetailsDto
            {
                Id = gateway.Id,
                Title = gateway.Title,
                ExpirationDate = gateway.ExpirationDate,
                IncomeUsageCount = incomeUsage,
                ExpenseUsageCount = expenseUsage,
                TotalIncome = income,
                TotalExpense = expense,
                CurrentBalance = gateway.InitialBalance + income - expense
            };
        }

        public async Task<List<PartnerPaymentGatewayStatsDto>> GetPaymentGatewayStats(Guid id)
        {
            return await _context.Set<Transaction>()
                .Where(t => t.TransactionPartnerId == id)
                .GroupBy(t => new { t.PaymentGatewayId, t.PaymentGateway.Title })
                .Select(g => new PartnerPaymentGatewayStatsDto
                {
                    PaymentGatewayId = g.Key.PaymentGatewayId,
                    Title = g.Key.Title,

                    UsageCount = g.Count(),

                    TotalIncome = g
                        .Where(x => EF.Property<string>(x, "Discriminator") == "Income")
                        .Sum(x => (decimal?)x.Amount) ?? 0,

                    TotalExpense = g
                        .Where(x => EF.Property<string>(x, "Discriminator") == "Expense")
                        .Sum(x => (decimal?)x.Amount) ?? 0
                })
                .ToListAsync();
        }

    }
}
