using Microsoft.EntityFrameworkCore;
using PersonalBudgetTrackerAPI.Models;

namespace PersonalBudgetTrackerAPI.DatabaseContext
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        public ApplicationDbContext()
        {

        }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<Category> Category { get; set; }

        public DbSet<PaymentGateway> PaymentGateway { get; set; }

        public DbSet<Reason> Reason { get; set; }
        public DbSet<TransactionPartner> TransactionPartner { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>()
            .HasDiscriminator<string>("Discriminator")
            .HasValue<Transaction>("Transaction")
            .HasValue<Income>("Income")
            .HasValue<Expense>("Expense");

            modelBuilder.Entity<Transaction>()
                .HasQueryFilter(t => !t.IsDeleted);

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

            foreach (var entry in entries)
            {
                var entity = (AuditableEntity)entry.Entity;

                var currentUser = "System";
                var now = DateTime.UtcNow;

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
