using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FamilyApi.DataModels
{
    public class Dish :BaseItem
    {
        [BsonElement("name")]
        public string? Name { get; set; }

        [BsonElement("photo")]
        public string? Photo { get; set; }
    }
}
