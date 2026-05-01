using Microsoft.EntityFrameworkCore;
using PersonalBudgetTrackerAPI.Common;
using PersonalBudgetTrackerAPI.Common.Exceptions;
using PersonalBudgetTrackerAPI.DatabaseContext;
using PersonalBudgetTrackerAPI.DTOs.Entities.TransactionDTOs;
using PersonalBudgetTrackerAPI.Models.Entities;
using PersonalBudgetTrackerAPI.Services.Interfaces;

namespace PersonalBudgetTrackerAPI.Services.Implementations
{
    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext _context;

        public TransactionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<TransactionDto> CreateAsync(CreateTransactionDto dto)
        {
            throw new NotImplementedException();
        }



        public Task<PagedResult<TransactionDto>> GetUserTransactionsAsync(TransactionFilterDto filter)
        {
            throw new NotImplementedException();
        }

        public async Task<TransactionDto> GetByIdAsync(Guid id)
        {
            var transaction = await _context.Set<Transaction>()
                .Include(t => t.PaymentGateway)
                .Include(t => t.TransactionPartner)
                .FirstOrDefaultAsync(t => t.TransactionId == id)
                ?? throw new NotFoundException("Transaction not found");

            return transaction.ToDto();
        }

        public async Task<TransactionRequirementsDto> GetRequirementsAsync()
        {
            var hasGateways = await _context.PaymentGateway.AnyAsync(p => !p.IsDeleted);
            var hasCategories = await _context.Category.AnyAsync(c => !c.IsDeleted);
            var hasPartners = await _context.TransactionPartner.AnyAsync(t => !t.IsDeleted);

            var missing = new List<string>();

            if (!hasGateways) missing.Add("PaymentGateway");
            if (!hasCategories) missing.Add("Category");
            if (!hasPartners) missing.Add("TransactionPartner");

            return new TransactionRequirementsDto
            {
                HasPaymentGateways = hasGateways,
                HasCategories = hasCategories,
                HasTransactionPartners = hasPartners,

                MissingResources = missing,

                IncomeRequirements = new
                {
                    Required = new[] { "Amount", "Title", "PaymentType", "PaymentGatewayId", "TransactionPartnerId", "ReasonId" },
                    Optional = new[] { "TransactionDetails", "Date" }
                },

                ExpenseRequirements = new
                {
                    Required = new[] { "Amount", "Title", "PaymentType", "PaymentGatewayId", "TransactionPartnerId", "CategoryId", "FeeAmount" },
                    Optional = new[] { "TransactionDetails", "Date" }
                }
            };
        }

        public async Task<List<TransactionSimpleDto>> GetByReasonIdAsync(Guid reasonId)
       => await _context.Set<Income>()
           .Where(x => x.ReasonId == reasonId)
           .ToSimpleDto()
           .ToListAsync();

        public async Task<List<TransactionSimpleDto>> GetByCategoryIdAsync(Guid categoryId)
            => await _context.Set<Expense>()
                .Where(x => x.CategoryId == categoryId)
                .ToSimpleDto()
                .ToListAsync();

        public async Task<List<TransactionSimpleDto>> GetByPaymentGatewayIdAsync(Guid id)
            => await _context.Set<Transaction>()
                .Where(x => x.PaymentGatewayId == id)
                .ToSimpleDto()
                .ToListAsync();

        public async Task<List<TransactionSimpleDto>> GetByPartnerIdAsync(Guid id)
            => await _context.Set<Transaction>()
                .Where(x => x.TransactionPartnerId == id)
                .ToSimpleDto()
                .ToListAsync();
    }
}
