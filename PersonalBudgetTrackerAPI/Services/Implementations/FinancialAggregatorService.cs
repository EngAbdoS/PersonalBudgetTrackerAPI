using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.DailySnapshot;
using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.FinanialRulesBaseDTOs;
using PersonalBudgetTrackerAPI.Models.FinancialPrefrances;
using PersonalBudgetTrackerAPI.Services.Interfaces;

namespace PersonalBudgetTrackerAPI.Services.Implementations
{
    public class FinancialAggregatorService : IFinancialAggregatorService
    {
        private readonly IDailySnapshotMongoService _mongo;
        private readonly IDaySnapshotService _redis;
        private readonly ICurrentUserService _currentUserService;

        private static DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);
        private static DateOnly Yesterday => Today.AddDays(-1);

        public FinancialAggregatorService(
            IDailySnapshotMongoService mongo,
            IDaySnapshotService redis,
            ICurrentUserService currentUserService)
        {
            _mongo = mongo;
            _redis = redis;
            _currentUserService = currentUserService;
        }


        public async Task<IEnumerable<UserDailySnapshotDto>> GetDailySnapshotsAsync( DateOnly from, DateOnly? to = null)
        {


            var userId = _currentUserService.UserId!;
            var effectiveTo = to ?? Today;
            var result = new List<UserDailySnapshotDto>();

            var mongoTo = effectiveTo < Today ? effectiveTo : Yesterday;
            if (from <= mongoTo)
            {
                var mongoSnapshots = await _mongo.GetSnapshotsRangeAsync(from, mongoTo);
                result.AddRange(mongoSnapshots);
            }

            if (effectiveTo > Yesterday)
            {
                var todaySnapshot = await _redis.GetTodaySnapshotAsync(userId);
                if (todaySnapshot is not null)
                    result.Add(new UserDailySnapshotDto
                    {
                        UserId = Guid.Parse(userId),
                        Date = Today,
                        Snapshot = todaySnapshot
                    });
            }

            return result.OrderBy(s => s.Date);
        }



        public async Task<decimal> AggregiateTotalExpense(RuleAggregiationInputDTO input)
        {
            var DailySnapshots = await GetDailySnapshotsAsync(input.From, input.To);
            var snapshots = DailySnapshots.Select(s => s.Snapshot);

            return input.TargetType switch
            {
                LimitTargetType.All or null =>
                    AggregateTotalExpense(snapshots, input.PaymentGatewayId),

                LimitTargetType.Category =>
                    AggregateTotalCategoryExpense(snapshots, input.CategoryId ?? Guid.Empty, input.PaymentGatewayId),

                LimitTargetType.TransactionPartner =>
                    AggregateTotalPartnerExpense(snapshots, input.TransactionPartnerId ?? Guid.Empty, input.PaymentGatewayId),

                _ => 0
            };


        }
        public async Task<decimal> AggregiateTotalIncome(DateOnly from, Guid? PaymentGatewayId, DateOnly? to = null)
        {
            var dailySnapshots = await GetDailySnapshotsAsync(from, to);
            var snapshots = dailySnapshots.Select(s => s.Snapshot);

            return PaymentGatewayId is null
                ? snapshots.Sum(s => s.TotalIncome)
                : snapshots.Sum(s =>
                    s.PaymentGateways
                     .GetValueOrDefault(PaymentGatewayId.ToString()!)?.TotalIncome ?? 0);
        }


        private static decimal AggregateTotalExpense(IEnumerable<DailySnapshot> snapshots,Guid? paymentGatewayId)
        {
            return paymentGatewayId is null
                ? snapshots.Sum(s => s.TotalExpense)
                : snapshots.Sum(s =>
                    s.PaymentGateways
                     .GetValueOrDefault(paymentGatewayId.ToString()!)?.TotalExpense ?? 0);
        }

        private static decimal AggregateTotalCategoryExpense(IEnumerable<DailySnapshot> snapshots, Guid categoryId, Guid? paymentGatewayId)
        {

            var catKey = categoryId.ToString();

            return paymentGatewayId is null
                ? snapshots.Sum(s =>
                    s.SpendingCategories.GetValueOrDefault(catKey))
                : snapshots.Sum(s =>
                    s.PaymentGateways
                     .GetValueOrDefault(paymentGatewayId.ToString()!)
                     ?.CategoriesSpendIn.GetValueOrDefault(catKey) ?? 0);
        }
        private static decimal AggregateTotalPartnerExpense( IEnumerable<DailySnapshot> snapshots, Guid partnerId, Guid? paymentGatewayId)
        {

            var partnerKey = partnerId.ToString()!;

            return paymentGatewayId is null
                ? snapshots.Sum(s =>
                    s.SpendingPartners.GetValueOrDefault(partnerKey))
                : snapshots.Sum(s =>
                    s.PaymentGateways
                     .GetValueOrDefault(paymentGatewayId.ToString()!)
                     ?.PartnersSpendWith.GetValueOrDefault(partnerKey) ?? 0);
        }

      
    }
}
