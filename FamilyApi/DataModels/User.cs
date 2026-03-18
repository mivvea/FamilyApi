using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FamilyApi.DataModels
{
    public class User
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string PasswordHash { get; set; }
    }
}
