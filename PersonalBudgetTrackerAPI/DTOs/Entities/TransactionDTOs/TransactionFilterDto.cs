using PersonalBudgetTrackerAPI.Common;
using PersonalBudgetTrackerAPI.Models.Entities;

namespace PersonalBudgetTrackerAPI.DTOs.Entities.TransactionDTOs
{
    public class TransactionFilterDto : PaginationQuery
    {
        public string? Type { get; set; }
        public string? Search { get; set; }

        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public PaymentType? PaymentType { get; set; }

        public Guid? PaymentGatewayId { get; set; }
        public Guid? TransactionPartnerId { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? ReasonId { get; set; }


    }
}
