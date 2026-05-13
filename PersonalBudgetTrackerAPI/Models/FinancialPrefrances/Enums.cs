namespace PersonalBudgetTrackerAPI.Models.FinancialPrefrances
{
        public enum ValueType { Percentage, StaticAmount }
        public enum ScopeType { TotalUser, SpecificGateway }
        public enum PeriodType { Daily, Weekly, Monthly, Quarterly , Yearly, Custom }
        public enum LimitTargetType { Category, TransactionPartner , All }
        public enum RecurrenceMode { OneTime, Recurring, Manual}
        public enum FinancialRuleType { SavingRule, MinimumBalanceRule, ExpenseLimitRule }
        public enum RuleStatus { Safe, NearLimit, Violated  }


    public static class PeriodTypeExtensions
    {
        private static readonly Dictionary<PeriodType, int> _order = new()
    {
        { PeriodType.Daily,     1 },
        { PeriodType.Weekly,    2 },
        { PeriodType.Monthly,   3 },
        { PeriodType.Quarterly, 4 },
        { PeriodType.Yearly,    5 },
        { PeriodType.Custom,    0 },
    };

        public static int Order(this PeriodType period) => _order[period];

        public static bool IsGreaterThan(this PeriodType recurrence, PeriodType period)

        => recurrence.Order() > period.Order();
    }
}
