using Microsoft.EntityFrameworkCore;
using PersonalBudgetTrackerAPI.BackgroundJobs.Interfaces;
using PersonalBudgetTrackerAPI.Common.Utilities;
using PersonalBudgetTrackerAPI.DatabaseContext;
using PersonalBudgetTrackerAPI.Models.FinancialPrefrances;

namespace PersonalBudgetTrackerAPI.BackgroundJobs.Jobs
{
    public class RuleActivationJob : IRuleActivationJob
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RuleActivationJob> _logger;

        public RuleActivationJob(ApplicationDbContext context, ILogger<RuleActivationJob> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            var now = DateTime.UtcNow;
            _logger.LogInformation("[RuleActivationJob] Starting at {Time}", now);

            var rulesToAdvance = await _context.FinancialRules
                .Where(r =>
                    r.IsActive &&
                    r.RecurrenceMode == RecurrenceMode.Recurring &&
                    r.PeriodEnd < now &&
                    r.ExpiresAt != null &&
                    r.ExpiresAt > now)
                .ToListAsync();

            if (!rulesToAdvance.Any())
            {
                _logger.LogInformation("[RuleActivationJob] No rules to advance.");
                return;
            }

            foreach (var rule in rulesToAdvance)
            {
                try
                {
                    var oldStart = rule.PeriodStart;
                    var oldEnd   = rule.PeriodEnd;

                    var (newStart, newEnd) = PeriodResolver.ResolveNext(rule, now);

                    // Safety check: new window must not exceed ExpiresAt
                    if (newStart >= rule.ExpiresAt)
                    {
                        _logger.LogInformation(
                            "[RuleActivationJob] Rule {RuleId} '{Title}' next window {NewStart} " +
                            "is at or after ExpiresAt {ExpiresAt} — skipping advance, will be deactivated next run.",
                            rule.Id, rule.Title, newStart, rule.ExpiresAt);
                        continue;
                    }

                    rule.PeriodStart = newStart;
                    rule.PeriodEnd   = newEnd;

                    _logger.LogInformation(
                        "[RuleActivationJob] Advanced rule {RuleId} '{Title}' " +
                        "from [{OldStart} → {OldEnd}] to [{NewStart} → {NewEnd}]",
                        rule.Id, rule.Title, oldStart, oldEnd, newStart, newEnd);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "[RuleActivationJob] Failed to advance rule {RuleId} '{Title}'",
                        rule.Id, rule.Title);
                }
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("[RuleActivationJob] Advanced {Count} rules.", rulesToAdvance.Count);
        }
    }
}