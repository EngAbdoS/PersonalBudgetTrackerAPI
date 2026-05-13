using Microsoft.EntityFrameworkCore;
using PersonalBudgetTrackerAPI.DatabaseContext;
using PersonalBudgetTrackerAPI.DTOs.Entities.TransactionDTOs;
using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.FinanialRulesBaseDTOs;
using PersonalBudgetTrackerAPI.Models.FinancialPrefrances;
using PersonalBudgetTrackerAPI.Services.Interfaces.FinanialPrefrances;

namespace PersonalBudgetTrackerAPI.Services.Implementations.FinanialPrefrances
{
    public class TransactionValidationService : ITransactionValidationService
    {
        private readonly ApplicationDbContext _db;
        private readonly IFinancialAggregatorService _aggregator;
        private readonly decimal _nearLimitPercentage;

        public TransactionValidationService(
            ApplicationDbContext db,
            IFinancialAggregatorService aggregator,
            IConfiguration configuration)
        {
            _db = db;
            _aggregator = aggregator;
            _nearLimitPercentage = configuration.GetValue<decimal>(
                "RuleValidation:NearLimitThresholdPercentage") / 100m;
        }

        public async Task<TransactionValidationResult> ValidateExpenseAsync(
    ExpenseTransactionValidationDto dto)
        {
            var result = new TransactionValidationResult();

            var rules = await _db.FinancialRules.Where(r => r.IsActive).ToListAsync<FinancialRule>();

            foreach (var rule in rules)
            {
                if (!RuleAppliesToTransaction(rule, dto))
                    continue;

                var from = DateOnly.FromDateTime(rule.PeriodStart!.Value);
                var to = DateOnly.FromDateTime(rule.PeriodEnd!.Value);

                var threshold = await ResolveThresholdAsync(rule, from, to);

                var violation = rule switch
                {
                    ExpenseLimitRule expenseRule =>
                        await CheckExpenseLimitAsync(expenseRule, dto, from, to, threshold),

                    MinimumBalanceRule balanceRule =>
                        await CheckMinimumBalanceAsync(balanceRule, dto, from, to, threshold),

                    _ => null
                };

                if (violation is not null)
                    result.Violations.Add(violation);
            }

            return result;
        }


        private static bool RuleAppliesToTransaction( FinancialRule rule, ExpenseTransactionValidationDto dto)
        {
            if (rule.ScopeType == ScopeType.SpecificGateway &&
                rule.PaymentGatewayId != dto.PaymentGatewayId)
                return false;

            if (rule is ExpenseLimitRule limit)
            {
                return limit.TargetType switch
                {
                    LimitTargetType.All => true,
                    LimitTargetType.Category => limit.CategoryId == dto.CategoryId,
                    LimitTargetType.TransactionPartner => limit.TransactionPartnerId == dto.TransactionPartnerId,
                    _ => false
                };
            }

            return true;
        }

        private async Task<decimal> ResolveThresholdAsync( FinancialRule rule, DateOnly from,DateOnly to)
        {
            if (rule.ValueType == Models.FinancialPrefrances.ValueType.StaticAmount)
                return rule.Value;

            // PercentageOfIncome
            var totalIncome = await _aggregator.AggregiateTotalIncome( from, rule.ScopeType == ScopeType.SpecificGateway
                    ? rule.PaymentGatewayId
                    : null,
                    to);

            return totalIncome * (rule.Value / 100m);
        }


        private async Task<RuleViolationDto?> CheckExpenseLimitAsync(
            ExpenseLimitRule rule,
            ExpenseTransactionValidationDto dto,
            DateOnly from,
            DateOnly to,
            decimal threshold)
        {
            var aggInput = new RuleAggregiationInputDTO
            {
                From = from,
                To = to,
                PaymentGatewayId = rule.ScopeType == ScopeType.SpecificGateway
                                           ? rule.PaymentGatewayId
                                           : null,
                TargetType = rule.TargetType,
                CategoryId = rule.CategoryId,
                TransactionPartnerId = rule.TransactionPartnerId
            };

            var currentTotal = await _aggregator.AggregiateTotalExpense(aggInput);
            var afterTransaction = currentTotal + dto.Amount;

            var status = DetermineStatus(currentTotal, afterTransaction, threshold);

            if (status == RuleStatus.Safe) return null;

            return new RuleViolationDto
            {
                RuleId = rule.Id,
                RuleType = rule.GetType().Name,
                Title = rule.Title,
                Threshold = threshold,
                CurrentTotal = currentTotal,
                AfterTransaction = afterTransaction,
                Status = status
            };
        }


        private async Task<RuleViolationDto?> CheckMinimumBalanceAsync(
            MinimumBalanceRule rule,
            ExpenseTransactionValidationDto dto,
            DateOnly from,
            DateOnly to,
            decimal threshold)
        {
            var currentBalance = await _aggregator.AggregateTotalBalance(
                from,
                rule.ScopeType == ScopeType.SpecificGateway
                    ? rule.PaymentGatewayId
                    : null,
                to);

            var afterTransaction = currentBalance - dto.Amount;

            var status = afterTransaction < threshold
                ? RuleStatus.Violated
                : currentBalance < threshold / _nearLimitPercentage
                    ? RuleStatus.NearLimit
                    : RuleStatus.Safe;

            if (status == RuleStatus.Safe) return null;

            return new RuleViolationDto
            {
                RuleId = rule.Id,
                RuleType = rule.GetType().Name,
                Title = rule.Title,
                Threshold = threshold,
                CurrentTotal = currentBalance,
                AfterTransaction = afterTransaction,
                Status = status
            };
        }


        private RuleStatus DetermineStatus(
            decimal currentTotal,
            decimal afterTransaction,
            decimal threshold)
        {
            if (afterTransaction > threshold)
                return RuleStatus.Violated;

            if (currentTotal >= threshold * _nearLimitPercentage)
                return RuleStatus.NearLimit;

            return RuleStatus.Safe;
        }

    }
}
