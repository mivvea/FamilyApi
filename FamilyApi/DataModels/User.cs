using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FamilyApi.DataModels
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public  string? Id { get; set; }
        public required string Name { get; set; }
        public required string PasswordHash { get; set; }
        public string? Photo { get; set; }
        public int? DarkMode { get; set; }
        public string? Background { get; set; }
    }
}
