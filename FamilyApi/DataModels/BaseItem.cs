using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FamilyApi.DataModels
{
    public abstract class BaseItem
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("name")]
        public string? Name { get; set; }

        [BsonElement("addedBy")]
        public string? AddedBy { get; set; }

        [BsonElement("photo")]
        public string? Photo { get; set; }
    }
}
