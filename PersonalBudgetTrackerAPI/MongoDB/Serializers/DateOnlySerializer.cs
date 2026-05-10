using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace PersonalBudgetTrackerAPI.MongoDB.Serializers
{
    public class DateOnlySerializer : StructSerializerBase<DateOnly>
    {
        public override void Serialize(
            BsonSerializationContext context,
            BsonSerializationArgs args,
            DateOnly value)
        {
            context.Writer.WriteString(value.ToString("yyyy-MM-dd"));
        }

        public override DateOnly Deserialize(
            BsonDeserializationContext context,
            BsonDeserializationArgs args)
        {
            return DateOnly.Parse(context.Reader.ReadString());
        }
    }
}
