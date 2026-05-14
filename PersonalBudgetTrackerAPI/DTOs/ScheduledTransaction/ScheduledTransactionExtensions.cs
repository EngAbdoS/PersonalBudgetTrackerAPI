using PersonalBudgetTrackerAPI.DTOs.ScheduledTransaction;
using PersonalBudgetTrackerAPI.Models.ScheduledTransaction;

namespace PersonalBudgetTrackerAPI.DTOs.Entities.ScheduledTransactionDTOs
{
    public static class ScheduledTransactionExtensions
    {
        public static ScheduledTransactionDto ToDto(this Models.ScheduledTransaction.ScheduledTransaction st) =>
            new()
            {
                Id = st.Id,
                Title = st.Title,
                TransactionDetails = st.TransactionDetails,
                Amount = st.Amount,
                IsIncome = st.IsIncome,
                IsFlexibleAmount = st.IsFlexibleAmount,
                PaymentType = st.PaymentType.ToString(),
                PaymentGatewayId = st.PaymentGatewayId,
                PaymentGatewayName = st.PaymentGateway?.Title ?? string.Empty,
                TransactionPartnerId = st.TransactionPartnerId,
                TransactionPartnerName = st.TransactionPartner?.Name ?? string.Empty,
                CategoryId = st.CategoryId,
                CategoryName = st.Category?.Title,
                FeeAmount = st.FeeAmount,
                ReasonId = st.ReasonId,
                ReasonDetails = st.Reason?.ReasonDetails,
                PeriodType = st.PeriodType.ToString(),
                DayOfWeek = st.DayOfWeek?.ToString(),
                DayOfMonth = st.DayOfMonth,
                NextDueDate = st.NextDueDate,
                IsActive = st.IsActive,
                CreatedAt = st.CreatedAt
            };

        public static ScheduledTransactionOccurrenceDto ToDto(
            this ScheduledTransactionOccurrence o, Models.ScheduledTransaction.ScheduledTransaction st) =>
            new()
            {
                Id = o.Id,
                ScheduledTransactionId = o.ScheduledTransactionId,
                Title = st.Title,
                Amount = st.Amount,
                IsFlexibleAmount = st.IsFlexibleAmount,
                IsIncome = st.IsIncome,
                DueDate = o.DueDate,
                Status = o.Status.ToString(),
                PaymentGatewayName = st.PaymentGateway?.Title ?? string.Empty,
                TransactionPartnerName = st.TransactionPartner?.Name ?? string.Empty,
                CategoryName = st.Category?.Title,
                ReasonDetails = st.Reason?.ReasonDetails
            };
    }
}