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

    }
}
