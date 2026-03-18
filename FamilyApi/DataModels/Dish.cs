using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FamilyApi.DataModels
{
    public class Dish
    {
        [BsonId] 
        public ObjectId Id { get; set; }

        [BsonElement("name")]
        public string? Name { get; set; }

        [BsonElement("photo")]
        public string? Photo { get; set; }

        [BsonElement("addedBy")]
        public string? AddedBy { get; set; }
    }
}
