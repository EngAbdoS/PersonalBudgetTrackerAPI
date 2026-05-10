namespace PersonalBudgetTrackerAPI.MongoDB.Settings
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string SnapshotsCollection { get; set; } = string.Empty;
    }
}
    