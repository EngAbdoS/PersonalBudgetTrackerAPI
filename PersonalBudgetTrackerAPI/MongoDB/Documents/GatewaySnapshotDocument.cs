using MongoDB.Bson.Serialization.Attributes;

namespace PersonalBudgetTrackerAPI.MongoDB.Documents
{
    public class GatewaySnapshotDocument
    {

        [BsonElement("totalIncome")]
        public decimal TotalIncome { get; set; }

        [BsonElement("totalExpense")]
        public decimal TotalExpense { get; set; }

        [BsonElement("categoriesSpendIn")]
        public Dictionary<string, decimal> CategoriesSpendIn { get; set; } = [];

        [BsonElement("partnersSpendWith")]
        public Dictionary<string, decimal> PartnersSpendWith { get; set; } = [];

    }
}
