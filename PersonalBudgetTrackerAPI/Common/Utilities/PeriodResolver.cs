using PersonalBudgetTrackerAPI.Common.Extentions;
using PersonalBudgetTrackerAPI.Models.FinancialPrefrances;

namespace PersonalBudgetTrackerAPI.Common.Utilities
{
    public static class PeriodResolver
    {
        public static (DateTime Start, DateTime End) Resolve(FinancialRule rule, DateTime now)
        {
            return rule.PeriodType switch
            {
                PeriodType.Weekly => GetWeek(now),
                PeriodType.Monthly => GetMonth(now),
                PeriodType.Quarterly => GetQuarter(now),
                PeriodType.Yearly => GetYear(now),
                PeriodType.Custom => GetCustom(rule),
                _ => throw new ArgumentOutOfRangeException(
                                            nameof(rule.PeriodType),
                                            rule.PeriodType,
                                            "Unsupported period type.")
            };
        }


        public static (DateTime Start, DateTime End) ResolveNext(FinancialRule rule, DateTime afterDate)
        {
            return rule.RecurrencePeriod switch
            {
                PeriodType.Weekly => GetWeek(afterDate.AddDays(1)),
                PeriodType.Monthly => GetMonth(afterDate.AddDays(1)),
                PeriodType.Quarterly => GetQuarter(afterDate.AddDays(1)),
                PeriodType.Yearly => GetYear(afterDate.AddDays(1)),
                null => Resolve(rule, afterDate.AddDays(1)), // fallback to PeriodType
                _ => throw new ArgumentOutOfRangeException(
                                            nameof(rule.RecurrencePeriod),
                                            rule.RecurrencePeriod,
                                            "Unsupported recurrence period type.")
            };
        }


        private static (DateTime Start, DateTime End) GetWeek(DateTime now)
        {
            var diff = ((int)now.DayOfWeek - (int)DayOfWeek.Saturday + 7) % 7;
            var start = now.AddDays(-diff).AtStartOfDay();
            var end = start.AddDays(6).AtEndOfDay();
            return (start, end);
        }

        private static (DateTime Start, DateTime End) GetMonth(DateTime now)
        {
            var start = new DateTime(now.Year, now.Month, 1).AtStartOfDay();
            var end = new DateTime(now.Year, now.Month,
                            DateTime.DaysInMonth(now.Year, now.Month)).AtEndOfDay();
            return (start, end);
        }

        private static (DateTime Start, DateTime End) GetQuarter(DateTime now)
        {
            // Q1: Jan-Mar  Q2: Apr-Jun  Q3: Jul-Sep  Q4: Oct-Dec
            var quarterStartMonth = ((now.Month - 1) / 3) * 3 + 1;
            var start = new DateTime(now.Year, quarterStartMonth, 1).AtStartOfDay();
            var end = start.AddMonths(3).AddDays(-1).AtEndOfDay();
            return (start, end);
        }

        private static (DateTime Start, DateTime End) GetYear(DateTime now)
        {
            var start = new DateTime(now.Year, 1, 1).AtStartOfDay();
            var end = new DateTime(now.Year, 12, 31).AtEndOfDay();
            return (start, end);
        }

        private static (DateTime Start, DateTime End) GetCustom(FinancialRule rule)
        {
            if (rule.PeriodStart is null || rule.PeriodEnd is null)
                throw new InvalidOperationException(
                    $"Rule '{rule.Title}' has PeriodType.Custom but PeriodStart or PeriodEnd is null.");

            if (rule.PeriodStart > rule.PeriodEnd)
                throw new InvalidOperationException(
                    $"Rule '{rule.Title}' has PeriodStart ({rule.PeriodStart}) after PeriodEnd ({rule.PeriodEnd}).");

            return (rule.PeriodStart.Value.AtStartOfDay(), rule.PeriodEnd.Value.AtEndOfDay());
        }
    }
}
