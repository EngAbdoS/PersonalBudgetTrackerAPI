using Microsoft.EntityFrameworkCore;
using PersonalBudgetTrackerAPI.BackgroundJobs.Interfaces;
using PersonalBudgetTrackerAPI.DatabaseContext;
using PersonalBudgetTrackerAPI.Models.FinancialPrefrances;

namespace PersonalBudgetTrackerAPI.BackgroundJobs.Jobs
{
    public class RuleDeactivationJob : IRuleDeactivationJob
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RuleDeactivationJob> _logger;

        public RuleDeactivationJob(ApplicationDbContext context, ILogger<RuleDeactivationJob> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            var now = DateTime.UtcNow;
            _logger.LogInformation("[RuleDeactivationJob] Starting at {Time}", now);

            var rulesToDeactivate = await _context.FinancialRules
                .Where(r => r.IsActive && r.PeriodEnd < now &&
                (
                    r.RecurrenceMode == RecurrenceMode.OneTime ||
                    r.RecurrenceMode == RecurrenceMode.Manual ||

                    // Recurring: ExpiresAt has passed
                    (r.RecurrenceMode == RecurrenceMode.Recurring && r.ExpiresAt != null && r.ExpiresAt < now)
                ))
                .ToListAsync();

            if (!rulesToDeactivate.Any())
            {
                _logger.LogInformation("[RuleDeactivationJob] No rules to deactivate.");
                return;
            }

            foreach (var rule in rulesToDeactivate)
            {
                rule.IsActive = false;
                _logger.LogInformation(
                    "[RuleDeactivationJob] Deactivated rule {RuleId} '{Title}' " +
                    "(Mode: {Mode}, PeriodEnd: {PeriodEnd}, ExpiresAt: {ExpiresAt})",
                    rule.Id, rule.Title, rule.RecurrenceMode, rule.PeriodEnd, rule.ExpiresAt);
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("[RuleDeactivationJob] Deactivated {Count} rules.", rulesToDeactivate.Count);
        }
    }
}