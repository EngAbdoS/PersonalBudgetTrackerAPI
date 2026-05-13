using Microsoft.EntityFrameworkCore;
using PersonalBudgetTrackerAPI.Common.Exceptions;
using PersonalBudgetTrackerAPI.Common.Pagination;
using PersonalBudgetTrackerAPI.DatabaseContext;
using PersonalBudgetTrackerAPI.DTOs.Entities.TransactionDTOs;
using PersonalBudgetTrackerAPI.Models.Entities;
using PersonalBudgetTrackerAPI.Services.Interfaces.Auth;
using PersonalBudgetTrackerAPI.Services.Interfaces.Entities;
using PersonalBudgetTrackerAPI.Services.Interfaces.FinanialPrefrances;

namespace PersonalBudgetTrackerAPI.Services.Implementations.Entities
{
    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITransactionPartnerService _partnerService;
        private readonly IReasonService _reasonService;
        private readonly ICategoryService _categoryService;
        private readonly IPaymentGatewayService _gatewayService;
        private readonly IDaySnapshotService _snapshotService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ITransactionValidationService _validationService;
        private readonly IPendingTransactionCacheService _pendingCache;

        public TransactionService(ApplicationDbContext context,
            ITransactionPartnerService partnerService,
            IReasonService reasonService,
            ICategoryService categoryService,
            IPaymentGatewayService gatewayService,
            IDaySnapshotService snapshotService,
            ICurrentUserService currentUserService,
            ITransactionValidationService validationService,
            IPendingTransactionCacheService pendingCache)
        {
            _context = context;
            _partnerService = partnerService;
            _reasonService = reasonService;
            _categoryService = categoryService;
            _gatewayService = gatewayService;
            _snapshotService = snapshotService;
            _currentUserService = currentUserService;
            _validationService = validationService;
            _pendingCache = pendingCache;
        }
        public async Task<CreateTransactionResponse> CreateAsync(CreateTransactionDto dto)
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
                if (!await _partnerService.TransactionPartnerValidAndExist(dto.TransactionPartnerId.Value))
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

                await ApplyTransactionAsync(income);

                return new CreateTransactionResponse { Transaction = income.ToDto() };
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

            // =========================
            // 5. VALIDATE AGAINST RULES
            // =========================
            var validationDto = new ExpenseTransactionValidationDto
            {
                Amount = dto.Amount,
                PaymentGatewayId = paymentGatewayId,
                CategoryId = categoryId,
                TransactionPartnerId = partnerId
            };

            var validationResult = await _validationService.ValidateExpenseAsync(validationDto);

            // =========================
            // 6. HANDLE VIOLATIONS
            // =========================
            if (!validationResult.IsValid)
            {
                // cache the transaction for user confirmation
                var cachedId = await _pendingCache.CacheAsync(dto);

                return new CreateTransactionResponse
                {
                    CachedTransactionId = cachedId,
                    RuleValidation = validationResult
                };
            }

            // =========================
            // 7. SAVE EXPENSE
            // =========================
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

            await ApplyTransactionAsync(expense);

            return new CreateTransactionResponse
            {
                Transaction = expense.ToDto(),
                RuleValidation = validationResult.Violations.Any()
                                    ? validationResult    
                                    : null
            };
        }

        public async Task<CreateTransactionResponse> ConfirmPendingTransactionAsync(Guid cachedId)
        {
            var dto = await _pendingCache.GetAsync(cachedId)
                ?? throw new NotFoundException("Pending transaction not found or expired");

            await _pendingCache.DeleteAsync(cachedId);

            var expense = new Expense
            {
                TransactionId = Guid.NewGuid(),
                Amount = dto.Amount,
                Title = dto.Title,
                TransactionDetails = dto.TransactionDetails,
                Date = dto.Date,
                PaymentType = dto.PaymentType,
                PaymentGatewayId = dto.PaymentGatewayId!.Value,
                TransactionPartnerId = dto.TransactionPartnerId!.Value,
                CategoryId = dto.CategoryId!.Value,
                FeeAmount = dto.FeeAmount ?? 0
            };

            await ApplyTransactionAsync(expense);

            return new CreateTransactionResponse { Transaction = expense.ToDto() };
        }

        public async Task<PagedResult<TransactionDto>> GetUserTransactionsAsync(TransactionFilterDto filter)
        {
            var query = _context.Set<Transaction>()
                .Include(t => t.TransactionPartner)
                .Include(t => t.PaymentGateway)
                .AsQueryable();

            //  SEARCH
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(t =>
                    t.Title.Contains(filter.Search) ||
                    (t.TransactionDetails != null && t.TransactionDetails.Contains(filter.Search)));
            }

            //  AMOUNT
            if (filter.MinAmount.HasValue)
                query = query.Where(t => t.Amount >= filter.MinAmount);

            if (filter.MaxAmount.HasValue)
                query = query.Where(t => t.Amount <= filter.MaxAmount);

            //  DATE
            if (filter.FromDate.HasValue)
                query = query.Where(t => t.Date >= filter.FromDate);

            if (filter.ToDate.HasValue)
                query = query.Where(t => t.Date <= filter.ToDate);

            //  PAYMENT
            if (filter.PaymentType.HasValue)
                query = query.Where(t => t.PaymentType == filter.PaymentType);

            if (filter.PaymentGatewayId.HasValue)
                query = query.Where(t => t.PaymentGatewayId == filter.PaymentGatewayId);

            // PARTNER
            if (filter.TransactionPartnerId.HasValue)
                query = query.Where(t => t.TransactionPartnerId == filter.TransactionPartnerId);

            // TYPE (Income / Expense)
            if (!string.IsNullOrWhiteSpace(filter.Type))
            {
                if (filter.Type.ToLower() == "income")
                    query = query.Where(t => EF.Property<string>(t, "Discriminator") == "Income");

                else if (filter.Type.ToLower() == "expense")
                    query = query.Where(t => EF.Property<string>(t, "Discriminator") == "Expense");
            }

            //  CATEGORY (only Expense)
            if (filter.CategoryId.HasValue)
            {
                query = query.Where(t =>
                    EF.Property<string>(t, "Discriminator") == "Expense" &&
                    ((Expense)t).CategoryId == filter.CategoryId);
            }

            //  REASON (only Income)
            if (filter.ReasonId.HasValue)
            {
                query = query.Where(t =>
                    EF.Property<string>(t, "Discriminator") == "Income" &&
                    ((Income)t).ReasonId == filter.ReasonId);
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(t => t.Date)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResult<TransactionDto>
            {
                Items = items.Select(t => t.ToDto()).ToList(),

                TotalCount = total,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }

        public async Task<TransactionDto> GetByIdAsync(Guid id)
        {
            var transaction = await _context.Set<Transaction>()
                .Include(t => t.PaymentGateway)
                .Include(t => t.TransactionPartner)
                .FirstOrDefaultAsync(t => t.TransactionId == id)
                ?? throw new NotFoundException("Transaction not found");

            if (transaction is Income income)
            {
                await _context.Entry(income)
                    .Reference(x => x.Reason)
                    .LoadAsync();
            }

            if (transaction is Expense expense)
            {
                await _context.Entry(expense)
                    .Reference(x => x.Category)
                    .LoadAsync();
            }

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


        private async Task ApplyTransactionAsync(Transaction transaction)
        {
            _context.Add(transaction);
            await _context.SaveChangesAsync();

            if (transaction is Income income)
            {
                _ = _snapshotService.ApplyIncomeAsync(
                    userId: _currentUserService.UserId!,
                    gatewayId: income.PaymentGatewayId,
                    partnerId: income.TransactionPartnerId,
                    amount: income.Amount,
                    transactionDate: DateTime.Now);
            }
            else if (transaction is Expense expense)
            {
                _ = _snapshotService.ApplyExpenseAsync(
                    userId: _currentUserService.UserId!,
                    gatewayId: expense.PaymentGatewayId,
                    categoryId: expense.CategoryId,
                    partnerId: expense.TransactionPartnerId,
                    amount: expense.Amount,
                    transactionDate: DateTime.Now);
            }
        }


    }
}
