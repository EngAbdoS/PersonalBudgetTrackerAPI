using Microsoft.EntityFrameworkCore;
using PersonalBudgetTrackerAPI.Common.Exceptions;
using PersonalBudgetTrackerAPI.Common.Pagination;
using PersonalBudgetTrackerAPI.Common.Utilities;
using PersonalBudgetTrackerAPI.DatabaseContext;
using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.ExpenseLimitRuleDTOs;
using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.FinanialRuleDTOs;
using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.FinanialRulesBaseDTOs;
using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.MinimumBalanceRuleDTOs;
using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.SavingRuleDTOs;
using PersonalBudgetTrackerAPI.Models.FinancialPrefrances;
using PersonalBudgetTrackerAPI.Services.Interfaces;
using System.Data;

namespace PersonalBudgetTrackerAPI.Services.Implementations
{
    public class FinanialRuleService : IFinanialRuleService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICategoryService _categoryService;
        private readonly ITransactionPartnerService _transactionPartnerService;
        private readonly IPaymentGatewayService _paymentGatewayService;
        private readonly IFinancialAggregatorService _financialAggregatorService;

        public FinanialRuleService(
            ApplicationDbContext context, 
            ICategoryService categoryService, 
            ITransactionPartnerService transactionPartnerService, 
            IPaymentGatewayService paymentGatewayService,
            IFinancialAggregatorService financialAggregatorService)
        {
            _context = context;
            _categoryService = categoryService;
            _transactionPartnerService = transactionPartnerService;
            _paymentGatewayService = paymentGatewayService;
            _financialAggregatorService = financialAggregatorService;
        }

        public async Task<ExpenseLimitRuleDto> CreateExpenseLimitRuleAsync(CreateExpenseLimitRuleDto dto)
        {
            if (dto.PaymentGatewayId.HasValue && !await _paymentGatewayService.PaymentGatewayValidAndExist(dto.PaymentGatewayId.Value))
                throw new NotFoundException("Payment gateway not found.");

            if (dto.CategoryId.HasValue && !await _categoryService.CategoryValidAndExist(dto.CategoryId.Value))
                throw new NotFoundException("Category not found.");

            if (dto.TransactionPartnerId.HasValue && !await _transactionPartnerService.TransactionPartnerValidAndExist(dto.TransactionPartnerId.Value))
                throw new NotFoundException("Transaction partner not found.");

            var rule = new ExpenseLimitRule
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Notes = dto.Notes,
                ValueType = dto.ValueType,
                Value = dto.Value,
                ScopeType = dto.ScopeType,
                PaymentGatewayId = dto.PaymentGatewayId,
                PeriodType = dto.PeriodType,
                PeriodStart = dto.PeriodStart,
                PeriodEnd = dto.PeriodEnd,
                RecurrenceMode = dto.RecurrenceMode,
                RecurrencePeriod = dto.RecurrencePeriod,
                TargetType = dto.TargetType,
                CategoryId = dto.CategoryId,
                TransactionPartnerId = dto.TransactionPartnerId,
                IsActive = true
            };

            if (rule.PeriodType != PeriodType.Custom)
            {
                var (start, end) = PeriodResolver.Resolve(rule, DateTime.UtcNow);
                rule.PeriodStart = start;
                rule.PeriodEnd = end;
            }

            _context.FinancialRules.Add(rule);
            await _context.SaveChangesAsync();

            var loadedRule = await _context.FinancialRules
                .OfType<ExpenseLimitRule>()
                .Include(r => r.PaymentGateway)
                .Include(r => r.Category)
                .Include(r => r.TransactionPartner)
                .FirstAsync(r => r.Id == rule.Id);

            return loadedRule.ToDto();
        }

        public async Task<MinimumBalanceRuleDto> CreateMinimumBalanceRuleAsync(CreateMinimumBalanceRuleDto dto)
        {
            if (dto.PaymentGatewayId.HasValue && !await _paymentGatewayService.PaymentGatewayValidAndExist(dto.PaymentGatewayId.Value))
                throw new NotFoundException("Payment gateway not found.");

            var rule = new MinimumBalanceRule
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Notes = dto.Notes,
                ValueType = dto.ValueType,
                Value = dto.Value,
                ScopeType = dto.ScopeType,
                PaymentGatewayId = dto.PaymentGatewayId,
                PeriodType = dto.PeriodType,
                PeriodStart = dto.PeriodStart,
                PeriodEnd = dto.PeriodEnd,
                RecurrenceMode = dto.RecurrenceMode,
                RecurrencePeriod = dto.RecurrencePeriod,
                IsActive = true
            };

            if (rule.PeriodType != PeriodType.Custom)
            {
                var (start, end) = PeriodResolver.Resolve(rule, DateTime.UtcNow);
                rule.PeriodStart = start;
                rule.PeriodEnd = end;
            }

            _context.FinancialRules.Add(rule);
            await _context.SaveChangesAsync();

            var loadedRule = await _context.FinancialRules
                .OfType<MinimumBalanceRule>()
                .Include(r => r.PaymentGateway)
                .FirstAsync(r => r.Id == rule.Id);

            return loadedRule.ToDto();
        }

        public async Task<SavingRuleDto> CreateSavingRuleAsync(CreateSavingRuleDto dto)
        {
            if (dto.PaymentGatewayId.HasValue && !await _paymentGatewayService.PaymentGatewayValidAndExist(dto.PaymentGatewayId.Value))
                throw new NotFoundException("Payment gateway not found.");

            var rule = new SavingRule
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Notes = dto.Notes,
                ValueType = dto.ValueType,
                Value = dto.Value,
                ScopeType = dto.ScopeType,
                PaymentGatewayId = dto.PaymentGatewayId,
                PeriodType = dto.PeriodType,
                PeriodStart = dto.PeriodStart,
                PeriodEnd = dto.PeriodEnd,
                RecurrenceMode = dto.RecurrenceMode,
                RecurrencePeriod = dto.RecurrencePeriod,
                IsActive = true
            };

            if (rule.PeriodType != PeriodType.Custom)
            {
                var (start, end) = PeriodResolver.Resolve(rule, DateTime.UtcNow);
                rule.PeriodStart = start;
                rule.PeriodEnd = end;
            }

            _context.FinancialRules.Add(rule);
            await _context.SaveChangesAsync();

            var loadedRule = await _context.FinancialRules
                .OfType<SavingRule>()
                .Include(r => r.PaymentGateway)
                .FirstAsync(r => r.Id == rule.Id);

            return loadedRule.ToDto();
        }

        public async Task<ExpenseLimitRuleDto> UpdateExpenseLimitRuleAsync(Guid id, UpdateExpenseLimitRuleDto dto)
        {
            if (dto.PaymentGatewayId.HasValue && !await _paymentGatewayService.PaymentGatewayValidAndExist(dto.PaymentGatewayId.Value))
                throw new NotFoundException("Payment gateway not found.");

            if (dto.CategoryId.HasValue && !await _categoryService.CategoryValidAndExist(dto.CategoryId.Value))
                throw new NotFoundException("Category not found.");

            if (dto.TransactionPartnerId.HasValue && !await _transactionPartnerService.TransactionPartnerValidAndExist(dto.TransactionPartnerId.Value))
                throw new NotFoundException("Transaction partner not found.");

            var rule = await _context.FinancialRules.OfType<ExpenseLimitRule>().FirstOrDefaultAsync(r => r.Id == id);
            if (rule == null)
                throw new NotFoundException("Expense limit rule not found.");

            if (dto.Title != null) rule.Title = dto.Title;
            if (dto.Notes != null) rule.Notes = dto.Notes;
            if (dto.ValueType.HasValue) rule.ValueType = dto.ValueType.Value;
            if (dto.Value.HasValue) rule.Value = dto.Value.Value;
            if (dto.ScopeType.HasValue) rule.ScopeType = dto.ScopeType.Value;
            
            rule.PaymentGatewayId = dto.PaymentGatewayId ?? rule.PaymentGatewayId;

            if (dto.PeriodType.HasValue) rule.PeriodType = dto.PeriodType.Value;
            
            rule.PeriodStart = dto.PeriodStart ?? rule.PeriodStart;
            rule.PeriodEnd = dto.PeriodEnd ?? rule.PeriodEnd;
            
            if (dto.RecurrenceMode.HasValue) rule.RecurrenceMode = dto.RecurrenceMode.Value;
            
            rule.RecurrencePeriod = dto.RecurrencePeriod ?? rule.RecurrencePeriod;
            
            if (dto.IsActive.HasValue) rule.IsActive = dto.IsActive.Value;
            
            if (dto.TargetType.HasValue) rule.TargetType = dto.TargetType.Value;
            
            rule.CategoryId = dto.CategoryId ?? rule.CategoryId;
            rule.TransactionPartnerId = dto.TransactionPartnerId ?? rule.TransactionPartnerId;

            if (rule.PeriodType != PeriodType.Custom)
            {
                var (start, end) = PeriodResolver.Resolve(rule, DateTime.UtcNow);
                rule.PeriodStart = start;
                rule.PeriodEnd = end;
            }

            await _context.SaveChangesAsync();

            var loadedRule = await _context.FinancialRules
                .OfType<ExpenseLimitRule>()
                .Include(r => r.PaymentGateway)
                .Include(r => r.Category)
                .Include(r => r.TransactionPartner)
                .FirstAsync(r => r.Id == rule.Id);

            return loadedRule.ToDto();
        }

        public async Task<MinimumBalanceRuleDto> UpdateMinimumBalanceRuleAsync(Guid id, UpdateMinimumBalanceRuleDto dto)
        {
            if (dto.PaymentGatewayId.HasValue && !await _paymentGatewayService.PaymentGatewayValidAndExist(dto.PaymentGatewayId.Value))
                throw new NotFoundException("Payment gateway not found.");

            var rule = await _context.FinancialRules.OfType<MinimumBalanceRule>().FirstOrDefaultAsync(r => r.Id == id);
            if (rule == null)
                throw new NotFoundException("Minimum balance rule not found.");

            if (dto.Title != null) rule.Title = dto.Title;
            if (dto.Notes != null) rule.Notes = dto.Notes;
            if (dto.ValueType.HasValue) rule.ValueType = dto.ValueType.Value;
            if (dto.Value.HasValue) rule.Value = dto.Value.Value;
            if (dto.ScopeType.HasValue) rule.ScopeType = dto.ScopeType.Value;
            
            rule.PaymentGatewayId = dto.PaymentGatewayId ?? rule.PaymentGatewayId;

            if (dto.PeriodType.HasValue) rule.PeriodType = dto.PeriodType.Value;
            
            rule.PeriodStart = dto.PeriodStart ?? rule.PeriodStart;
            rule.PeriodEnd = dto.PeriodEnd ?? rule.PeriodEnd;

            if (dto.RecurrenceMode.HasValue) rule.RecurrenceMode = dto.RecurrenceMode.Value;
            
            rule.RecurrencePeriod = dto.RecurrencePeriod ?? rule.RecurrencePeriod;

            if (dto.IsActive.HasValue) rule.IsActive = dto.IsActive.Value;

            if (rule.PeriodType != PeriodType.Custom)
            {
                var (start, end) = PeriodResolver.Resolve(rule, DateTime.UtcNow);
                rule.PeriodStart = start;
                rule.PeriodEnd = end;
            }

            await _context.SaveChangesAsync();

            var loadedRule = await _context.FinancialRules
                .OfType<MinimumBalanceRule>()
                .Include(r => r.PaymentGateway)
                .FirstAsync(r => r.Id == rule.Id);

            return loadedRule.ToDto();
        }

        public async Task<SavingRuleDto> UpdateSavingRuleAsync(Guid id, UpdateSavingRuleDto dto)
        {
            if (dto.PaymentGatewayId.HasValue && !await _paymentGatewayService.PaymentGatewayValidAndExist(dto.PaymentGatewayId.Value))
                throw new NotFoundException("Payment gateway not found.");

            var rule = await _context.FinancialRules.OfType<SavingRule>().FirstOrDefaultAsync(r => r.Id == id);
            if (rule == null)
                throw new NotFoundException("Saving rule not found.");

            if (dto.Title != null) rule.Title = dto.Title;
            if (dto.Notes != null) rule.Notes = dto.Notes;
            if (dto.ValueType.HasValue) rule.ValueType = dto.ValueType.Value;
            if (dto.Value.HasValue) rule.Value = dto.Value.Value;
            if (dto.ScopeType.HasValue) rule.ScopeType = dto.ScopeType.Value;
            
            rule.PaymentGatewayId = dto.PaymentGatewayId ?? rule.PaymentGatewayId;

            if (dto.PeriodType.HasValue) rule.PeriodType = dto.PeriodType.Value;
            
            rule.PeriodStart = dto.PeriodStart ?? rule.PeriodStart;
            rule.PeriodEnd = dto.PeriodEnd ?? rule.PeriodEnd;

            if (dto.RecurrenceMode.HasValue) rule.RecurrenceMode = dto.RecurrenceMode.Value;
            
            rule.RecurrencePeriod = dto.RecurrencePeriod ?? rule.RecurrencePeriod;

            if (dto.IsActive.HasValue) rule.IsActive = dto.IsActive.Value;

            if (rule.PeriodType != PeriodType.Custom)
            {
                var (start, end) = PeriodResolver.Resolve(rule, DateTime.UtcNow);
                rule.PeriodStart = start;
                rule.PeriodEnd = end;
            }

            await _context.SaveChangesAsync();

            var loadedRule = await _context.FinancialRules
                .OfType<SavingRule>()
                .Include(r => r.PaymentGateway)
                .FirstAsync(r => r.Id == rule.Id);

            return loadedRule.ToDto();
        }

        public async Task<PagedResult<FinancialRuleBaseDto>> GetUserRulesAsync(FinancialRuleFilterDto filter)
        {
            var query = _context.FinancialRules.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(r => r.Title.Contains(filter.Search) || (r.Notes != null && r.Notes.Contains(filter.Search)));
            }

            if (filter.ScopeType.HasValue) query = query.Where(r => r.ScopeType == filter.ScopeType.Value);
            if (filter.PeriodType.HasValue) query = query.Where(r => r.PeriodType == filter.PeriodType.Value);
            if (filter.RecurrenceMode.HasValue) query = query.Where(r => r.RecurrenceMode == filter.RecurrenceMode.Value);
            if (filter.ValueType.HasValue) query = query.Where(r => r.ValueType == filter.ValueType.Value);
            if (filter.IsActive.HasValue) query = query.Where(r => r.IsActive == filter.IsActive.Value);
            if (filter.PaymentGatewayId.HasValue) query = query.Where(r => r.PaymentGatewayId == filter.PaymentGatewayId.Value);

            if (filter.MinValue.HasValue) query = query.Where(r => r.Value >= filter.MinValue.Value);
            if (filter.MaxValue.HasValue) query = query.Where(r => r.Value <= filter.MaxValue.Value);

            if (filter.StartDateFrom.HasValue) query = query.Where(r => r.PeriodStart >= filter.StartDateFrom.Value);
            if (filter.StartDateTo.HasValue) query = query.Where(r => r.PeriodStart <= filter.StartDateTo.Value);
            
            if (filter.EndDateFrom.HasValue) query = query.Where(r => r.PeriodEnd >= filter.EndDateFrom.Value);
            if (filter.EndDateTo.HasValue) query = query.Where(r => r.PeriodEnd <= filter.EndDateTo.Value);

            if (filter.RuleType.HasValue)
            {
                query = filter.RuleType.Value switch
                {
                    FinancialRuleType.SavingRule => query.OfType<SavingRule>(),
                    FinancialRuleType.MinimumBalanceRule => query.OfType<MinimumBalanceRule>(),
                    FinancialRuleType.ExpenseLimitRule => query.OfType<ExpenseLimitRule>(),
                    _ => query
                };
            }

            var totalCount = await query.CountAsync();
            if (totalCount < 1) throw new NotFoundException("No rules found matching the criteria.");

            query = query.OrderByDescending(r => r.CreatedAt);
            var items = await query.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize)
                                   .Include(r => r.PaymentGateway)
                                   .ToListAsync();

            var dtos = new List<FinancialRuleBaseDto>();
            foreach (var item in items)
            {
                if (item is ExpenseLimitRule elr)
                {
                    if (elr.CategoryId.HasValue)
                        await _context.Entry(elr).Reference(e => e.Category).LoadAsync();
                    if (elr.TransactionPartnerId.HasValue)
                        await _context.Entry(elr).Reference(e => e.TransactionPartner).LoadAsync();
                        
                    dtos.Add(elr.ToDto());
                }
                else if (item is MinimumBalanceRule mbr)
                {
                    dtos.Add(mbr.ToDto());
                }
                else if (item is SavingRule sr)
                {
                    dtos.Add(sr.ToDto());
                }


            }

            return new PagedResult<FinancialRuleBaseDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }
        public async Task ActivateRuleAsync(Guid id)
        {
            var rule = await _context.FinancialRules.FirstOrDefaultAsync(r => r.Id == id);
            if (rule == null) throw new NotFoundException("Rule not found.");

            rule.IsActive = true;
            await _context.SaveChangesAsync();
        }

        public async Task DeactivateRuleAsync(Guid id)
        {
            var rule = await _context.FinancialRules.FirstOrDefaultAsync(r => r.Id == id);
            if (rule == null) throw new NotFoundException("Rule not found.");

            rule.IsActive = false;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> RuleExistsAndActiveAsync(Guid ruleId)
        {
            var now = DateTime.UtcNow;
            return await _context.FinancialRules.AnyAsync(r =>
                r.Id == ruleId &&
                r.IsActive &&
                (r.PeriodStart == null || r.PeriodStart <= now) &&
                (r.PeriodEnd == null || r.PeriodEnd >= now) &&
                (r.ExpiresAt == null || r.ExpiresAt >= now));
        }
        public async Task<List<SavingGoalStatusDto>> GetSavingGoalsStatusAsync()
        {

            var savingRules = await _context.FinancialRules
                                       .OfType<SavingRule>()
                                       .Where(r => r.IsActive)
                                       .Include(r => r.PaymentGateway)
                                       .ToListAsync();

            if (!savingRules.Any())
                throw new NotFoundException("No active saving goals found.");

            var result = new List<SavingGoalStatusDto>();

            foreach (var rule in savingRules)
            {
                var ruleDto = rule.ToDto(); 

                var aggregationInput = ruleDto.ToAggregationInput();

                var from = DateOnly.FromDateTime(rule.PeriodStart ?? DateTime.UtcNow);
                var to = DateOnly.FromDateTime(rule.PeriodEnd ?? DateTime.UtcNow);

                decimal currentSaved = await _financialAggregatorService.AggregateTotalBalance(
                    from,
                    rule.PaymentGatewayId,
                    to
                );

                decimal targetAmount = rule.ValueType == Models.FinancialPrefrances.ValueType.Percentage
                    ? await ResolvePercentageTargetAsync(rule)   
                    : rule.Value;

                decimal remaining = Math.Max(0, targetAmount - currentSaved);
                decimal progress = targetAmount > 0
                    ? Math.Min(100, (currentSaved / targetAmount) * 100)
                    : 0;

                result.Add(new SavingGoalStatusDto
                {
                    RuleId = rule.Id,
                    Title = rule.Title,
                    TargetAmount = targetAmount,
                    CurrentSavedAmount = currentSaved,
                    RemainingAmount = remaining,
                    ProgressPercentage = Math.Round(progress, 2),
                    IsCompleted = currentSaved >= targetAmount,
                    TargetDate = rule.PeriodEnd ?? rule.ExpiresAt
                });
            }

            return result;

        }

        private async Task<decimal> ResolvePercentageTargetAsync(SavingRule rule)
        {
            decimal totalIncome = await _financialAggregatorService.AggregiateTotalIncome(
                DateOnly.FromDateTime(rule.PeriodStart ?? DateTime.UtcNow),
                rule.PaymentGatewayId,
                DateOnly.FromDateTime(rule.PeriodEnd ?? DateTime.UtcNow)
            );

            return Math.Round(totalIncome * (rule.Value / 100m), 2);
        }

    }
}
