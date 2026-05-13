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
                PeriodType.Daily => GetDay(now),
                PeriodType.Weekly => GetWeek(now),
                PeriodType.Monthly => GetMonth(now),
                PeriodType.Quarterly => GetQuarter(now),
                PeriodType.Yearly => GetYear(now),
                PeriodType.Custom => GetCustom(rule),
                _ => throw new ArgumentOutOfRangeException(nameof(rule.PeriodType), rule.PeriodType,
                         "Unsupported period type.")
            };
        }

        public static (DateTime Start, DateTime End) Resolve(PeriodType periodType, DateTime now,
            DateTime? customStart = null, DateTime? customEnd = null)
        {
            return periodType switch
            {
                PeriodType.Daily => GetDay(now),
                PeriodType.Weekly => GetWeek(now),
                PeriodType.Monthly => GetMonth(now),
                PeriodType.Quarterly => GetQuarter(now),
                PeriodType.Yearly => GetYear(now),
                PeriodType.Custom => GetCustomFromDates(customStart, customEnd),
                _ => throw new ArgumentOutOfRangeException(nameof(periodType), periodType,
                         "Unsupported period type.")
            };
        }

        public static (DateTime Start, DateTime End) ResolveNext(FinancialRule rule, DateTime afterDate)
        {
            if (rule.PeriodType == PeriodType.Custom)
                return GetNextCustomWindow(rule, afterDate);

            return rule.RecurrencePeriod switch
            {
                PeriodType.Daily => GetDay(afterDate.AddDays(1)),
                PeriodType.Weekly => GetWeek(afterDate.AddDays(1)),
                PeriodType.Monthly => GetMonth(afterDate.AddDays(1)),
                PeriodType.Quarterly => GetQuarter(afterDate.AddDays(1)),
                PeriodType.Yearly => GetYear(afterDate.AddDays(1)),
                null => Resolve(rule, afterDate.AddDays(1)),
                _ => throw new ArgumentOutOfRangeException(nameof(rule.RecurrencePeriod),
                         rule.RecurrencePeriod, "Unsupported recurrence period type.")
            };
        }
        public static DateTime? ResolveExpiresAt(PeriodType? recurrencePeriod, DateTime now)
        {
            if (recurrencePeriod is null)
                return null;

            return recurrencePeriod switch
            {
                PeriodType.Daily => GetDay(now.AddDays(1)).Start,
                PeriodType.Weekly => GetWeek(now.AddDays(7)).Start,
                PeriodType.Monthly => GetMonth(now.AddMonths(1)).Start,
                PeriodType.Quarterly => GetQuarter(now.AddMonths(3)).Start,
                PeriodType.Yearly => GetYear(now.AddYears(1)).Start,
                _ => throw new ArgumentOutOfRangeException(nameof(recurrencePeriod), recurrencePeriod,
                         "Unsupported recurrence period type for ExpiresAt calculation.")
            };
        }

        private static (DateTime Start, DateTime End) GetDay(DateTime now)
        {
            var start = now.Date.AtStartOfDay();
            return (start, start.AddDays(1).AddTicks(-1));
        }

        private static (DateTime Start, DateTime End) GetWeek(DateTime now)
        {
            var diff = ((int)now.DayOfWeek - (int)DayOfWeek.Saturday + 7) % 7;
            var start = now.AddDays(-diff).AtStartOfDay();
            return (start, start.AddDays(6).AtEndOfDay());
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
            var quarterStartMonth = ((now.Month - 1) / 3) * 3 + 1;
            var start = new DateTime(now.Year, quarterStartMonth, 1).AtStartOfDay();
            return (start, start.AddMonths(3).AddDays(-1).AtEndOfDay());
        }

        private static (DateTime Start, DateTime End) GetYear(DateTime now)
        {
            var start = new DateTime(now.Year, 1, 1).AtStartOfDay();
            return (start, new DateTime(now.Year, 12, 31).AtEndOfDay());
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

        private static (DateTime Start, DateTime End) GetCustomFromDates(
            DateTime? customStart, DateTime? customEnd)
        {
            if (customStart is null || customEnd is null)
                throw new InvalidOperationException(
                    "PeriodStart and PeriodEnd are required for Custom period type.");

            if (customStart > customEnd)
                throw new InvalidOperationException(
                    "PeriodStart must be before PeriodEnd for Custom period type.");

            return (customStart.Value.AtStartOfDay(), customEnd.Value.AtEndOfDay());
        }
        private static (DateTime Start, DateTime End) GetNextCustomWindow(FinancialRule rule, DateTime afterDate)
        {
            if (rule.PeriodStart is null || rule.PeriodEnd is null)
                throw new InvalidOperationException(
                    $"Rule '{rule.Title}' has PeriodType.Custom but PeriodStart or PeriodEnd is null.");

            var duration = rule.PeriodEnd.Value - rule.PeriodStart.Value;
            var nextStart = afterDate.AddDays(1).AtStartOfDay();
            var nextEnd = nextStart.Add(duration).AtEndOfDay();

            return (nextStart, nextEnd);
        }
    }
  

}
