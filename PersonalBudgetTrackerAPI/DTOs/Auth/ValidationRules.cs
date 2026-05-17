namespace PersonalBudgetTrackerAPI.DTOs.Auth
{
    public static class ValidationRules
    {
         public static bool BeValidTimeZone(string? timeZone)
        {
            if (string.IsNullOrWhiteSpace(timeZone))
                return true;

            return TimeZoneInfo
                .GetSystemTimeZones()
                .Any(tz => tz.Id == timeZone);
        }

        public static readonly string[] SupportedCurrencies =
            {
                "EGP",
                "USD",
                "EUR"
            };

        public static readonly string[] SupportedLanguages = ["en", "ar"];
    }
}
