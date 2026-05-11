using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.DailySnapshot;
using PersonalBudgetTrackerAPI.DTOs.FinanialPrefrancesDTOs.FinanialRulesBaseDTOs;
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



        public Task<decimal> AggregiateTotalExpense(RuleAggregiationInputDTO inputDTO)
        {
            throw new NotImplementedException();
        }
    }
}
