using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PersonalBudgetTrackerAPI.MongoDB.Documents
{
    public class MonthlySnapshotDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; } = string.Empty;        // "{userId}:{yyyy-MM}"

        [BsonElement("userId")]
        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }

        [BsonElement("month")]
        public string Month { get; set; } = string.Empty;     // "2025-05"

        [BsonElement("days")]
        public Dictionary<string, DailySnapshotDocument> Days { get; set; } = [];

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}
