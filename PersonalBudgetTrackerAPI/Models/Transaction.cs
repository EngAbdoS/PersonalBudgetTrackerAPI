using System.ComponentModel.DataAnnotations;

namespace PersonalBudgetTrackerAPI.Models
{
    public class Transaction : AuditableEntity
    {
        [Key]
        [Required]
        public Guid TransactionId { get; set; }
        [Required]
        public required decimal Amount { get; set; }
        [Required]
        public required string Title { get; set; }
        public string? TransactionDetails { get; set; }

        public DateTime Date { get; set; }

      //  public TransactionType TransactionType { get; set; }
        public Guid TransactionPartnerId { get; set; }
        public TransactionPartner TransactionPartner { get; set; } = null!;
        [Required]


        public required PaymentType PaymentType { get; set; }
        public Guid PaymentGatewayId { get; set; }

        public PaymentGateway PaymentGateway { get; set; } = null!;

    }
    /*
    public enum TransactionType
    {
        Income,
        Expense
    }
    */
    public enum PaymentType
    {
        Cash,
        Digital
    }

}
