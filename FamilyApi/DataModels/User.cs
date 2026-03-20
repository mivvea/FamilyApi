using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FamilyApi.DataModels
{
    public class User
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public required string Name { get; set; }
        public required string PasswordHash { get; set; }
        public string? Photo { get; set; }
    }
}
