using System.ComponentModel.DataAnnotations;

namespace PersonalBudgetTrackerAPI.Models.Entities
{
    public class PaymentGateway : AuditableEntity
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required ]
        public required string Title { get; set; }
        [Required]

        public required string Description { get; set; }
        [Required]

        public required string BankName { get; set; }

        [Required]
        public PaymentGatewayType PaymentGatewayType { get; set; }

    }

    public enum PaymentGatewayType
    {
        Visa = 1,
        MasterCard = 2,
        PayPal = 3,
        ApplePay = 4,
        GooglePay = 5,
        Wallet = 6,
        Other = 7
    }
}
