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
        private readonly ITransactionPartnerService _partnerService;
        private readonly IReasonService _reasonService;
        private readonly ICategoryService _categoryService;
        private readonly IPaymentGatewayService _gatewayService;

        public TransactionService(ApplicationDbContext context,
            ITransactionPartnerService partnerService,
            IReasonService reasonService,
            ICategoryService categoryService,
            IPaymentGatewayService gatewayService)
        {
            _context = context;
            _partnerService = partnerService;
            _reasonService = reasonService;
            _categoryService = categoryService;
            _gatewayService = gatewayService;
        }
        public async Task<TransactionDto> CreateAsync(CreateTransactionDto dto)
        {
            // =========================
            // 1. PAYMENT GATEWAY
            // =========================
            Guid paymentGatewayId;

            if (dto.PaymentGatewayId.HasValue)
            {
                if (!await _gatewayService.PaymentGatewayValidAndExist(dto.PaymentGatewayId.Value))
                    throw new NotFoundException("Invalid payment gateway");

                paymentGatewayId = dto.PaymentGatewayId.Value;
            }
            else if (dto.NewPaymentGateway != null)
            {
                var gateway = await _gatewayService.CreateAsync(dto.NewPaymentGateway);
                paymentGatewayId = gateway.Id;
            }
            else
                throw new BadRequestException("Payment gateway is required");


            // =========================
            // 2. TRANSACTION PARTNER
            // =========================
            Guid partnerId;

            if (dto.TransactionPartnerId.HasValue)
            {
                if(await _partnerService.TransactionPartnerValidAndExist(dto.TransactionPartnerId.Value) == false)
                    throw new NotFoundException("Invalid transaction partner");
                partnerId = dto.TransactionPartnerId.Value;

            }
            else if (dto.NewPartner != null)
            {
                var partner = await _partnerService.CreateAsync(dto.NewPartner);
                partnerId = partner.Id;
            }
            else
                throw new BadRequestException("Transaction partner is required");

            // =========================
            // 3. INCOME FLOW
            // =========================
            if (dto.IsIncome)
            {
                Guid reasonId;

                if (dto.ReasonId.HasValue)
                {
                    if (!await _reasonService.ReasonValidAndExist(dto.ReasonId.Value))
                        throw new NotFoundException("Invalid reason");

                    reasonId = dto.ReasonId.Value;
                }
                else if (!string.IsNullOrWhiteSpace(dto.NewReason))
                {
                    var reason = await _reasonService.CreateReasonAsync(dto.NewReason);
                    reasonId = reason.ReasonId;
                }
                else
                    throw new BadRequestException("Reason is required for income");

                var income = new Income
                {
                    TransactionId = Guid.NewGuid(),
                    Amount = dto.Amount,
                    Title = dto.Title,
                    TransactionDetails = dto.TransactionDetails,
                    Date = dto.Date,

                    PaymentType = dto.PaymentType,
                    PaymentGatewayId = paymentGatewayId,
                    TransactionPartnerId = partnerId,

                    ReasonId = reasonId
                };

                _context.Add(income);

                await _context.SaveChangesAsync();

                return income.ToDto();
            }

            // =========================
            // 4. EXPENSE FLOW
            // =========================
            Guid categoryId;

            if (dto.CategoryId.HasValue)
            {
                if (!await _categoryService.CategoryValidAndExist(dto.CategoryId.Value))
                    throw new NotFoundException("Invalid category");

                categoryId = dto.CategoryId.Value;
            }
            else if (dto.NewCategory != null)
            {
                var category = await _categoryService.CreateCategoryAsync(dto.NewCategory);
                categoryId = category.CategoryId;
            }
            else
                throw new BadRequestException("Category is required for expense");

            var expense = new Expense
            {
                TransactionId = Guid.NewGuid(),
                Amount = dto.Amount,
                Title = dto.Title,
                TransactionDetails = dto.TransactionDetails,
                Date = dto.Date,

                PaymentType = dto.PaymentType,
                PaymentGatewayId = paymentGatewayId,
                TransactionPartnerId = partnerId,

                CategoryId = categoryId,
                FeeAmount = dto.FeeAmount ?? 0
            };

            _context.Add(expense);

            await _context.SaveChangesAsync();

            return expense.ToDto();
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
