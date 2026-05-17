using Microsoft.AspNetCore.Identity;

namespace PersonalBudgetTrackerAPI.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string LanguageCode { get; set; } = "en";
        public string CurrencyCode { get; set; } = "EGP";
        public string TimeZone { get; set; } = "Africa/Cairo";

        public bool IsDarkMode { get; set; }

        public bool IsOnboardingCompleted { get; set; }
        public DateTime RegisteredAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? LastSeenAtUtc { get; set; }

        public bool PushNotificationsEnabled { get; set; } = true;

        public string? ProfileImageUrl { get; set; }

        public bool IsPremium { get; set; }
        public DateTime? PremiumExpiresAtUtc { get; set; }

    }
}
