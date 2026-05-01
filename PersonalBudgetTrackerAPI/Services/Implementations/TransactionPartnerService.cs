using Microsoft.EntityFrameworkCore;
using PersonalBudgetTrackerAPI.Common.Exceptions;
using PersonalBudgetTrackerAPI.Common.Pagination;
using PersonalBudgetTrackerAPI.DatabaseContext;
using PersonalBudgetTrackerAPI.DTOs.Entities.TransactionPartnerDTOs;
using PersonalBudgetTrackerAPI.Models.Entities;
using PersonalBudgetTrackerAPI.Services.Interfaces;


namespace PersonalBudgetTrackerAPI.Services.Implementations
{
    public class TransactionPartnerService : ITransactionPartnerService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPaymentGatewayService _paymentGatewayService; 
        public TransactionPartnerService(ApplicationDbContext context, IPaymentGatewayService paymentGatewayService)
        {
            _context = context;
            _paymentGatewayService = paymentGatewayService;
        }

        public async Task<bool> TransactionPartnerValidAndExist(Guid id)
        {
            return await _context.TransactionPartner
                .AnyAsync(p => p.Id == id && !p.IsDeleted);
        }


        public async Task<TransactionPartnerDto> CreateAsync(CreateTransactionPartnerDto dto)
        {
            var partner = new TransactionPartner
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Info = dto.Info,
                Location = dto.Location,
                Contact = dto.Contact
            };

            _context.TransactionPartner.Add(partner);
            await _context.SaveChangesAsync();

            return partner.ToDto();
        }

        public async Task<TransactionPartnerDto> UpdateAsync(Guid id, UpdateTransactionPartnerDto dto)
        {
            var entity = await _context.TransactionPartner.FindAsync(id)
                ?? throw new NotFoundException("Transaction partner not found");

            entity.Name = dto.Name;
            entity.Info = dto.Info;
            entity.Location = dto.Location;
            entity.Contact = dto.Contact;

            await _context.SaveChangesAsync();

            return entity.ToDto();
        }


        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.TransactionPartner.FindAsync(id)
                ?? throw new NotFoundException("Transaction partner not found");

            var hasTransactions = await _context.Set<Transaction>()
                .AnyAsync(t => t.TransactionPartnerId == id);

            if (hasTransactions)
                throw new BadRequestException("Cannot delete partner with transactions");

            _context.TransactionPartner.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResult<TransactionPartnerDto>> GetAllAsync(int page, int pageSize)
        {
            var query = _context.TransactionPartner
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => x.ToDto());

            var totalCount = await query.CountAsync();

            if (totalCount < 1)
            {
                throw new NotFoundException("No transaction partners found");
            }

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<TransactionPartnerDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<TransactionPartnerDetailsDto> GetDetailsAsync(Guid id)
        {
            var partner = await GetPartnerOrThrow(id);

            var usage = await GetUsageCount(id);
            var income = await GetTotalIncome(id);
            var expense = await GetTotalExpense(id);

            var categories = await GetCategories(id);
            var reasons = await GetReasons(id);
            var gateways = await _paymentGatewayService.GetPaymentGatewayStats(id);

            return new TransactionPartnerDetailsDto
            {
                Id = partner.Id,
                Name = partner.Name,
                UsageCount = usage,
                TotalIncome = income,
                TotalExpense = expense,
                Categories = categories,
                Reasons = reasons,
                PaymentGateways = gateways
            };
        }

        public async Task<List<TransactionPartnerDto>> GetPartnersByReasonIdAsync(Guid reasonId)
        {
            var partners = await _context.Set<Income>()
                .Where(i => i.ReasonId == reasonId)
                .Select(i => i.TransactionPartner)
                .Distinct()
                .ToListAsync();

            return partners.Select(p => p.ToDto()).ToList();
        }

        public async Task<List<TransactionPartnerDto>> GetPartnersByCategoryIdAsync(Guid categoryId)
        {
            var partners = await _context.Set<Expense>()
                .Where(e => e.CategoryId == categoryId)
                .Select(e => e.TransactionPartner)
                .Distinct()
                .ToListAsync();

            return partners.Select(p => p.ToDto()).ToList();
        }

        private async Task<TransactionPartner> GetPartnerOrThrow(Guid id)
        {
            return await _context.TransactionPartner
                .FirstOrDefaultAsync(p => p.Id == id)
                ?? throw new NotFoundException("Transaction partner not found");
        }
        private async Task<int> GetUsageCount(Guid id)
        {
            return await _context.Set<Transaction>()
                .CountAsync(t => t.TransactionPartnerId == id);
        }
        private async Task<decimal> GetTotalIncome(Guid id)
        {
            return await _context.Set<Income>()
                .Where(i => i.TransactionPartnerId == id)
                .SumAsync(i => (decimal?)i.Amount) ?? 0;
        }
        private async Task<decimal> GetTotalExpense(Guid id)
        {
            return await _context.Set<Expense>()
                .Where(e => e.TransactionPartnerId == id)
                .SumAsync(e => (decimal?)e.Amount) ?? 0;
        }
        private async Task<List<string>> GetCategories(Guid id)
        {
            return await _context.Set<Expense>()
                .Where(e => e.TransactionPartnerId == id)
                .Select(e => e.Category.Title)
                .Distinct()
                .ToListAsync();
        }
        private async Task<List<string>> GetReasons(Guid id)
        {
            return await _context.Set<Income>()
                .Where(i => i.TransactionPartnerId == id)
                .Select(i => i.Reason.ReasonDetails)
                .Distinct()
                .ToListAsync();
        }




    }
}
