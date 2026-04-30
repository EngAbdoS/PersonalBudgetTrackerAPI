using Microsoft.EntityFrameworkCore;
using PersonalBudgetTrackerAPI.Common;
using PersonalBudgetTrackerAPI.Common.Exceptions;
using PersonalBudgetTrackerAPI.DatabaseContext;
using PersonalBudgetTrackerAPI.DTOs.Entities.TransactionPartnerDTOs;
using PersonalBudgetTrackerAPI.Models.Entities;
using PersonalBudgetTrackerAPI.Services.Interfaces;


namespace PersonalBudgetTrackerAPI.Services.Implementations
{
    public class TransactionPartnerService : ITransactionPartnerService
    {
        private readonly ApplicationDbContext _context;
        public TransactionPartnerService(ApplicationDbContext context)
        {
            _context = context;
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






        public Task<TransactionPartnerDetailsDto> GetDetailsAsync(Guid id)
        {
            throw new NotImplementedException();
        }

     

       
    }
}
