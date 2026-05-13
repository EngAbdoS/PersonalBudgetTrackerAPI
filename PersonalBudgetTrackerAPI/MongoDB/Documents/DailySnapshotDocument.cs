using MongoDB.Bson.Serialization.Attributes;

namespace PersonalBudgetTrackerAPI.MongoDB.Documents
{
    public class DailySnapshotDocument
    {
        [BsonElement("totalTransactions")]
        public int TotalTransactions { get; set; }

        [BsonElement("totalIncome")]
        public decimal TotalIncome { get; set; }

        [BsonElement("totalExpense")]
        public decimal TotalExpense { get; set; }

        [BsonElement("paymentGateways")]
        public Dictionary<string, GatewaySnapshotDocument> PaymentGateways { get; set; } = [];

        [BsonElement("spendingCategories")]
        public Dictionary<string, decimal> SpendingCategories { get; set; } = [];

        [BsonElement("spendingPartners")]
        public Dictionary<string, decimal> SpendingPartners { get; set; } = [];

        [BsonElement("incomeFromPartners")]
        public Dictionary<string, decimal> IncomeFromPartners { get; set; } = [];

    }
}
