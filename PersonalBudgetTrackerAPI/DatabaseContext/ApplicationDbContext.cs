using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PersonalBudgetTrackerAPI.Identity;
using PersonalBudgetTrackerAPI.Models.Entities;
using PersonalBudgetTrackerAPI.Models.FinancialPrefrances;
using PersonalBudgetTrackerAPI.Models.ScheduledTransaction;
using PersonalBudgetTrackerAPI.Services.Interfaces.Auth;

namespace PersonalBudgetTrackerAPI.DatabaseContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly ICurrentUserService _currentUserService;


        public ApplicationDbContext(DbContextOptions options, ICurrentUserService currentUserService) : base(options)
        {
            _currentUserService = currentUserService;
        }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<Category> Category { get; set; }

        public DbSet<PaymentGateway> PaymentGateway { get; set; }

        public DbSet<Reason> Reason { get; set; }
        public DbSet<TransactionPartner> TransactionPartner { get; set; }

        public DbSet<FinancialRule> FinancialRules { get; set; }

        public DbSet<ScheduledTransaction> ScheduledTransactions { get; set; }
        public DbSet<ScheduledTransactionOccurrence> ScheduledTransactionOccurrences { get; set; }  // ← new

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>()
            .HasDiscriminator<string>("Discriminator")
            .HasValue<Transaction>("Transaction")
            .HasValue<Income>("Income")
            .HasValue<Expense>("Expense");

            modelBuilder.Entity<ScheduledTransaction>().ToTable("ScheduledPayments");

            modelBuilder.Entity<ScheduledTransactionOccurrence>(entity =>
            {
                entity.HasKey(o => o.Id);

                entity.HasOne(o => o.ScheduledTransaction)
                    .WithMany(st => st.Occurrences)
                    .HasForeignKey(o => o.ScheduledTransactionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<FinancialRule>()
                .HasDiscriminator<string>("RuleType")
                .HasValue<FinancialRule>("FinancialRule")  
                .HasValue<SavingRule>("SavingGoal")
                .HasValue<MinimumBalanceRule>("MinimumBalance")
                .HasValue<ExpenseLimitRule>("ExpenseLimit");


            modelBuilder.Entity<Transaction>()
                .HasQueryFilter(t => !t.IsDeleted && t.CreatedBy == _currentUserService.UserId); 
            
            modelBuilder.Entity<ScheduledTransaction>()
                .HasQueryFilter(st => !st.IsDeleted && st.CreatedBy == _currentUserService.UserId);

            modelBuilder.Entity<Category>()
                .HasQueryFilter(c => !c.IsDeleted && c.CreatedBy == _currentUserService.UserId);

            modelBuilder.Entity<PaymentGateway>()
                .HasQueryFilter(p => !p.IsDeleted && p.CreatedBy == _currentUserService.UserId);

            modelBuilder.Entity<Reason>()
                .HasQueryFilter(r => !r.IsDeleted && r.CreatedBy == _currentUserService.UserId);

            modelBuilder.Entity<TransactionPartner>()
                .HasQueryFilter(tp => !tp.IsDeleted && tp.CreatedBy == _currentUserService.UserId);

            modelBuilder.Entity<FinancialRule>()
                .HasQueryFilter(fr => !fr.IsDeleted && fr.CreatedBy == _currentUserService.UserId);

            base.OnModelCreating(modelBuilder);
        }
        public override int SaveChanges()
        {
            ApplyAuditInfo();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInfo();
            return base.SaveChangesAsync(cancellationToken);
        }


        private void ApplyAuditInfo()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is AuditableEntity &&
                           (e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted));

            var currentUser = _currentUserService.UserId ?? "SYSTEM";
            var now = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                var entity = (AuditableEntity)entry.Entity;
             
                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = now;
                    entity.CreatedBy = currentUser;
                }

                if (entry.State == EntityState.Modified)
                {
                    entity.UpdatedAt = now;
                    entity.UpdatedBy = currentUser;
                }

                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified; // Soft delete
                    entity.IsDeleted = true;
                    entity.DeletedAt = now;
                    entity.DeletedBy = currentUser;
                }
            }
        }

    }
}
