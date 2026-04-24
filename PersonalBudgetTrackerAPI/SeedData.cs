using PersonalBudgetTrackerAPI.DatabaseContext;
using PersonalBudgetTrackerAPI.Models;
using PersonalBudgetTrackerAPI.Models.Entities;

namespace PersonalBudgetTrackerAPI
{
    public class SeedData
    {
        public static void Seed(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            context.Database.EnsureCreated();


            if (!context.Category.Any()) // seeding some categories 
            {
                context.Category.AddRange(
                    new Category
                    {
                        Title = "Food",
                        Details = "snacks",
                        IsNeedful = false,
                        NeedPriority = 0.5M,
                    },

                    new Category
                    {
                        Title = "Transportation",
                        Details = "home to work",
                        IsNeedful = true,
                        NeedPriority = 1M,
                    }

                    );
                context.SaveChanges();


            }

            if (!context.PaymentGateway.Any()) // seeding some PaymentGateway 
            {
                context.PaymentGateway.AddRange(
                    new PaymentGateway
                    {
                        BankName = "Cairo bank",
                        PaymentGatewayType = PaymentGatewayType.Wallet,
                        Description = "my wallet for daly use ",
                        Title = "Telda"

                    },
                        new PaymentGateway
                        {
                            BankName = "CIB bank",
                            PaymentGatewayType = PaymentGatewayType.MasterCard,
                            Description = "my work card",
                            Title = "CIB card"

                        }


                    );

                context.SaveChanges();


            }

            if (!context.TransactionPartner.Any()) // seeding some TransactionPartner 
            {

                var foodCategory = context.Category.First(c => c.Title == "Food");
                var transportCategory = context.Category.First(c => c.Title == "Transportation");

                context.TransactionPartner.AddRange(
                    new TransactionPartner
                    {
                        Id = Guid.NewGuid(),
                        Name = "McDonald's",
                        Info = "Fast food restaurant",
                        Location = "Nasr City",
                        Contact = "123456789",
                        CategoryId = foodCategory.Id
                    },
                    new TransactionPartner
                    {
                        Id = Guid.NewGuid(),
                        Name = "Uber",
                        Info = "Ride service",
                        Location = "Cairo",
                        Contact = "987654321",
                        CategoryId = transportCategory.Id
                    },
                            new TransactionPartner
                            {
                                Id = Guid.NewGuid(),
                                Name = "work manager",
                                Info = "work financial manager",
                                Location = "Cairo",
                                Contact = "987654321",
                                CategoryId = transportCategory.Id
                            }
                );

                context.SaveChanges();
            }

            if (!context.Reason.Any()) // seeding some Reason 
            {
                context.Reason.Add(
                    new Reason
                    {
                        Id = Guid.NewGuid(),
                        ReasonDetails = "this is my salary"

                    }
                    );


                context.SaveChanges();
            }
            if (!context.Set<Transaction>().Any()) // seeding some Transactions
            {

                var foodCategory = context.Category.First(c => c.Title == "Food");
                var transportCategory = context.Category.First(c => c.Title == "Transportation");

                var telda = context.PaymentGateway.First(p => p.Title == "Telda");
                var cib = context.PaymentGateway.First(p => p.Title == "CIB Card");

                var mcdonalds = context.TransactionPartner.First(t => t.Name == "McDonald's");
                var uber = context.TransactionPartner.First(t => t.Name == "Uber");

                var salaryReason = context.Reason.First(r => r.ReasonDetails == "this is my salary");



                context.AddRange(

       // Expense Example
       new Expense
       {
           TransactionId = Guid.NewGuid(),
           Amount = 150,
           Title = "Dinner",
           TransactionDetails = "Dinner with friends",
           Date = DateTime.Now,
           PaymentType = PaymentType.Digital,
           PaymentGatewayId = telda.Id,
           TransactionPartnerId = mcdonalds.Id,
           CategoryId = foodCategory.Id,
           FeeAmount = 5
       },

       new Expense
       {
           TransactionId = Guid.NewGuid(),
           Amount = 80,
           Title = "Uber Ride",
           TransactionDetails = "Work ride",
           Date = DateTime.Now,
           PaymentType = PaymentType.Digital,
           PaymentGatewayId = telda.Id,
           TransactionPartnerId = uber.Id,
           CategoryId = transportCategory.Id,
           FeeAmount = 2
       },

       // Income Example
       new Income
       {
           TransactionId = Guid.NewGuid(),
           Amount = 10000,
           Title = "Salary March",
           TransactionDetails = "Monthly salary",
           Date = DateTime.Now,
           PaymentType = PaymentType.Digital,
           PaymentGatewayId = cib.Id,
           TransactionPartnerId = mcdonalds.Id,
           Reason = salaryReason
       }
   );

                context.SaveChanges();



            }

        }

    }



}

