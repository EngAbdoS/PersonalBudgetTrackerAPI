using Microsoft.EntityFrameworkCore;
using PersonalBudgetTrackerAPI.Common.Exceptions;
using PersonalBudgetTrackerAPI.Common.Pagination;
using PersonalBudgetTrackerAPI.DatabaseContext;
using PersonalBudgetTrackerAPI.DTOs.Entities.ScheduledTransactionDTOs;
using PersonalBudgetTrackerAPI.DTOs.Entities.TransactionDTOs;
using PersonalBudgetTrackerAPI.DTOs.ScheduledTransaction;
using PersonalBudgetTrackerAPI.Models.Entities;
using PersonalBudgetTrackerAPI.Models.FinancialPrefrances;
using PersonalBudgetTrackerAPI.Models.ScheduledTransaction;
using PersonalBudgetTrackerAPI.Services.Interfaces.Auth;
using PersonalBudgetTrackerAPI.Services.Interfaces.Entities;

namespace PersonalBudgetTrackerAPI.Services.Implementations.Entities
{
    public class ScheduledTransactionService : IScheduledTransactionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPaymentGatewayService _gatewayService;
        private readonly ITransactionPartnerService _partnerService;
        private readonly ICategoryService _categoryService;
        private readonly IReasonService _reasonService;
        private readonly ITransactionService _transactionService;
        private readonly ICurrentUserService _currentUserService;

        public ScheduledTransactionService(
            ApplicationDbContext context,
            IPaymentGatewayService gatewayService,
            ITransactionPartnerService partnerService,
            ICategoryService categoryService,
            IReasonService reasonService,
            ITransactionService transactionService,
            ICurrentUserService currentUserService)
        {
            _context = context;
            _gatewayService = gatewayService;
            _partnerService = partnerService;
            _categoryService = categoryService;
            _reasonService = reasonService;
            _transactionService = transactionService;
            _currentUserService = currentUserService;
        }

        public async Task<ScheduledTransactionDto> CreateAsync(CreateScheduledTransactionDto dto)
        {
            if (!await _gatewayService.PaymentGatewayValidAndExist(dto.PaymentGatewayId))
                throw new NotFoundException("Payment gateway not found.");

            if (!await _partnerService.TransactionPartnerValidAndExist(dto.TransactionPartnerId))
                throw new NotFoundException("Transaction partner not found.");

            if (!dto.IsIncome)
            {
                if (dto.CategoryId is null || !await _categoryService.CategoryValidAndExist(dto.CategoryId.Value))
                    throw new NotFoundException("Category not found.");
            }
            else
            {
                if (dto.ReasonId is null || !await _reasonService.ReasonValidAndExist(dto.ReasonId.Value))
                    throw new NotFoundException("Reason not found.");
            }

            var firstDueDate = ComputeFirstDueDate(dto);

            var scheduled = new ScheduledTransaction
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                TransactionDetails = dto.TransactionDetails,
                Amount = dto.Amount,
                IsIncome = dto.IsIncome,
                IsFlexibleAmount = dto.IsFlexibleAmount,
                PaymentType = dto.PaymentType,
                PaymentGatewayId = dto.PaymentGatewayId,
                TransactionPartnerId = dto.TransactionPartnerId,
                CategoryId = dto.CategoryId,
                FeeAmount = dto.FeeAmount,
                ReasonId = dto.ReasonId,
                PeriodType = dto.PeriodType,
                DayOfWeek = dto.DayOfWeek,
                DayOfMonth = dto.DayOfMonth,
                NextDueDate = firstDueDate,
                IsActive = true
            };

            _context.ScheduledTransactions.Add(scheduled);
            await _context.SaveChangesAsync();

            await _context.Entry(scheduled).Reference(s => s.PaymentGateway).LoadAsync();
            await _context.Entry(scheduled).Reference(s => s.TransactionPartner).LoadAsync();
            if (scheduled.CategoryId.HasValue)
                await _context.Entry(scheduled).Reference(s => s.Category).LoadAsync();
            if (scheduled.ReasonId.HasValue)
                await _context.Entry(scheduled).Reference(s => s.Reason).LoadAsync();

            return scheduled.ToDto();
        }


        public async Task<ScheduledTransactionDto> GetByIdAsync(Guid id)
        {
            var st = await _context.ScheduledTransactions
                .Include(s => s.PaymentGateway)
                .Include(s => s.TransactionPartner)
                .Include(s => s.Category)
                .Include(s => s.Reason)
                .FirstOrDefaultAsync(s => s.Id == id)
                ?? throw new NotFoundException("Scheduled transaction not found.");

            return st.ToDto();
        }

       
        public async Task<PagedResult<ScheduledTransactionDto>> GetAllAsync(ScheduledTransactionFilterDto filter)
        {
            var query = _context.ScheduledTransactions
                .Include(s => s.PaymentGateway)
                .Include(s => s.TransactionPartner)
                .Include(s => s.Category)
                .Include(s => s.Reason)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Search))
                query = query.Where(s => s.Title.Contains(filter.Search) ||
                                         (s.TransactionDetails != null && s.TransactionDetails.Contains(filter.Search)));

            if (filter.IsIncome.HasValue)
                query = query.Where(s => s.IsIncome == filter.IsIncome);

            if (filter.IsActive.HasValue)
                query = query.Where(s => s.IsActive == filter.IsActive);

            if (filter.IsFlexibleAmount.HasValue)
                query = query.Where(s => s.IsFlexibleAmount == filter.IsFlexibleAmount);

            if (filter.PeriodType.HasValue)
                query = query.Where(s => s.PeriodType == filter.PeriodType);

            if (filter.PaymentGatewayId.HasValue)
                query = query.Where(s => s.PaymentGatewayId == filter.PaymentGatewayId);

            if (filter.CategoryId.HasValue)
                query = query.Where(s => s.CategoryId == filter.CategoryId);

            if (filter.ReasonId.HasValue)
                query = query.Where(s => s.ReasonId == filter.ReasonId);

            if (filter.DueDateFrom.HasValue)
                query = query.Where(s => s.NextDueDate >= filter.DueDateFrom);

            if (filter.DueDateTo.HasValue)
                query = query.Where(s => s.NextDueDate <= filter.DueDateTo);

            if (filter.CreatedFrom.HasValue)
                query = query.Where(s => s.CreatedAt >= filter.CreatedFrom);

            if (filter.CreatedTo.HasValue)
                query = query.Where(s => s.CreatedAt <= filter.CreatedTo);

            var total = await query.CountAsync();

            var page = filter.Page < 1 ? 1 : filter.Page;
            var pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;

            var items = await query
                .OrderByDescending(s => s.NextDueDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ScheduledTransactionDto>
            {
                Items = items.Select(s => s.ToDto()).ToList(),
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

   
        public async Task DeactivateAsync(Guid id)
        {
            var st = await _context.ScheduledTransactions.FindAsync(id)
                ?? throw new NotFoundException("Scheduled transaction not found.");

            st.IsActive = false;
            await _context.SaveChangesAsync();
        }

       
        public async Task<IEnumerable<ScheduledTransactionOccurrenceDto>> GetPendingOccurrencesAsync()
        {
            var now = DateTime.UtcNow;
            var userId = _currentUserService.UserId;

            var items = await _context.ScheduledTransactionOccurrences
                .Include(o => o.ScheduledTransaction)
                    .ThenInclude(st => st.PaymentGateway)
                .Include(o => o.ScheduledTransaction)
                    .ThenInclude(st => st.TransactionPartner)
                .Include(o => o.ScheduledTransaction)
                    .ThenInclude(st => st.Category)
                .Include(o => o.ScheduledTransaction)
                    .ThenInclude(st => st.Reason)
                .Where(o => o.ScheduledTransaction.CreatedBy == userId &&
                            !o.ScheduledTransaction.IsDeleted &&
                            o.DueDate > now &&
                            (o.Status == OccurrenceStatus.Pending || o.Status == OccurrenceStatus.Seen))
                .OrderBy(o => o.DueDate)
                .ToListAsync();

            return items.Select(o => o.ToDto(o.ScheduledTransaction));  
           
        }

        public async Task<CreateTransactionResponse> ConfirmOccurrenceAsync(
            Guid occurrenceId, ConfirmScheduledTransactionDto? dto)
        {
            var occurrence = await LoadOccurrenceAsync(occurrenceId);

            if (occurrence.Status == OccurrenceStatus.Confirmed)
                throw new BadRequestException("This occurrence has already been confirmed.");

            if (occurrence.Status == OccurrenceStatus.Skipped)
                throw new BadRequestException("This occurrence was skipped and cannot be confirmed.");

            var st = occurrence.ScheduledTransaction;

            if (st.IsFlexibleAmount)
            {
                if (dto?.Amount is null or <= 0)
                    throw new BadRequestException(
                        "This scheduled transaction has a flexible amount. Please provide Amount in the request body.");
            }

            var resolvedAmount = st.IsFlexibleAmount ? dto!.Amount!.Value : st.Amount;

            var createDto = new CreateTransactionDto
            {
                Amount = resolvedAmount,
                Title = st.Title,
                TransactionDetails = st.TransactionDetails,
                Date = occurrence.DueDate.ToLocalTime(),   
                PaymentType = st.PaymentType,
                IsIncome = st.IsIncome,
                PaymentGatewayId = st.PaymentGatewayId,
                TransactionPartnerId = st.TransactionPartnerId,
                // Income
                ReasonId = st.ReasonId,
                // Expense
                CategoryId = st.CategoryId,
                FeeAmount = st.FeeAmount
            };

            var result = await _transactionService.CreateAsync(createDto);

            if (result.Transaction is not null)
            {
                occurrence.Status = OccurrenceStatus.Confirmed;
                await _context.SaveChangesAsync();
            }

            return result;
        }


        public async Task SkipOccurrenceAsync(Guid occurrenceId)
        {
            var occurrence = await LoadOccurrenceAsync(occurrenceId);

            if (occurrence.Status == OccurrenceStatus.Confirmed)
                throw new BadRequestException("Cannot skip an already confirmed occurrence.");

            if (occurrence.Status == OccurrenceStatus.Skipped)
                throw new BadRequestException("This occurrence is already skipped.");

            occurrence.Status = OccurrenceStatus.Skipped;
            await _context.SaveChangesAsync();
        }


        public async Task MarkAsSeenAsync(Guid occurrenceId)
        {
            var occurrence = await LoadOccurrenceAsync(occurrenceId);

            if (occurrence.Status != OccurrenceStatus.Pending)
                throw new BadRequestException("Only Pending occurrences can be marked as Seen.");

            occurrence.Status = OccurrenceStatus.Seen;
            await _context.SaveChangesAsync();
        }


        private async Task<ScheduledTransactionOccurrence> LoadOccurrenceAsync(Guid occurrenceId)
        {
            var userId = _currentUserService.UserId;

            var occurrence = await _context.ScheduledTransactionOccurrences
                .Include(o => o.ScheduledTransaction)
                    .ThenInclude(st => st.PaymentGateway)
                .Include(o => o.ScheduledTransaction)
                    .ThenInclude(st => st.TransactionPartner)
                .Include(o => o.ScheduledTransaction)
                    .ThenInclude(st => st.Category)
                .Include(o => o.ScheduledTransaction)
                    .ThenInclude(st => st.Reason)
                .FirstOrDefaultAsync(o => o.Id == occurrenceId &&
                                          o.ScheduledTransaction.CreatedBy == userId &&
                                          !o.ScheduledTransaction.IsDeleted)
                ?? throw new NotFoundException("Occurrence not found.");

            return occurrence;
        }

        
        private static DateTime ComputeFirstDueDate(CreateScheduledTransactionDto dto)
        {
            var now = DateTime.UtcNow;

            return dto.PeriodType switch
            {
                PeriodType.Daily =>
                    now.Date.AddDays(1).AddHours(8),

                PeriodType.Weekly =>
                    GetNextWeekday(now, dto.DayOfWeek!.Value),

                PeriodType.Monthly =>
                    GetNextMonthDay(now, dto.DayOfMonth!.Value),

                _ => throw new InvalidOperationException($"Unsupported PeriodType: {dto.PeriodType}")
            };
        }

        private static DateTime GetNextWeekday(DateTime from, DayOfWeek target)
        {
            var daysUntil = ((int)target - (int)from.DayOfWeek + 7) % 7;
            if (daysUntil == 0) daysUntil = 7; 
            return from.Date.AddDays(daysUntil).AddHours(8);
        }

        private static DateTime GetNextMonthDay(DateTime from, int dayOfMonth)
        {
            var candidate = new DateTime(from.Year, from.Month,
                Math.Min(dayOfMonth, DateTime.DaysInMonth(from.Year, from.Month)),
                8, 0, 0, DateTimeKind.Utc);

            if (candidate <= from)
            {
                var nextMonth = from.AddMonths(1);
                candidate = new DateTime(nextMonth.Year, nextMonth.Month,
                    Math.Min(dayOfMonth, DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month)),
                    8, 0, 0, DateTimeKind.Utc);
            }

            return candidate;
        }
    }
}
