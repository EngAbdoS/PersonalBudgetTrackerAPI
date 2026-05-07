namespace PersonalBudgetTrackerAPI.Common.Extentions
{
    public static class DateTimeExtensions
    {
        public static DateTime AtStartOfDay(this DateTime date) =>
            date.Date;

        public static DateTime AtEndOfDay(this DateTime date) =>
            date.Date.AddDays(1).AddMilliseconds(-1);
    }
}
