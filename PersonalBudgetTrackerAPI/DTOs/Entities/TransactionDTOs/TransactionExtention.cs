using Microsoft.EntityFrameworkCore;
using PersonalBudgetTrackerAPI.Models.Entities;

namespace PersonalBudgetTrackerAPI.DTOs.Entities.TransactionDTOs
{
    public static class TransactionExtention
    {
        public static TransactionDto ToDto(this Transaction t)
        {
            return new TransactionDto
            {
                TransactionId = t.TransactionId,
                Amount = t.Amount,
                Title = t.Title,
                TransactionDetails = t.TransactionDetails,
                Date = t.Date,
                PaymentType = t.PaymentType,
                Type = t is Income ? "Income" : "Expense",

                PaymentGateway = t.PaymentGateway?.Title ?? "",
                TransactionPartner = t.TransactionPartner?.Name ?? "",

                Reason = t is Income i ? i.Reason?.ReasonDetails : null,
                Category = t is Expense e ? e.Category?.Title : null
            };
        }
        public static IQueryable<TransactionSimpleDto> ToSimpleDto(this IQueryable<Transaction> query)
        {
            return query.Select(t => new TransactionSimpleDto
            {
                TransactionId = t.TransactionId,
                Amount = t.Amount,
                Title = t.Title,
                Type = EF.Property<string>(t, "Discriminator")
            });
        }
    }
}
