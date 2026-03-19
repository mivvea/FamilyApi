using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FamilyApi.DataModels
{
    public abstract class BaseItem
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("addedBy")]
        public string? AddedBy { get; set; }
    }
}
