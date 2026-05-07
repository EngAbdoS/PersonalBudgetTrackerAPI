namespace PersonalBudgetTrackerAPI.Models.FinancialPrefrances
{
        public enum ValueType { Percentage, StaticAmount }
        public enum ScopeType { TotalUser, SpecificGateway }
        public enum PeriodType { Daily, Weekly, Monthly, Quarterly , Yearly, Custom }
        public enum LimitTargetType { Category, TransactionPartner , All }
        public enum RecurrenceMode { OneTime, Recurring, Manual}
        public enum RuleType { SavingRule, MinimumBalanceRule, ExpenseLimitRule }
}
