using MongoDB.Bson;

namespace FamilyApi.DataModels
{
    public class Session
    {
        public ObjectId Id { get; set; }
        public string UserId { get; set; }
        public string Jwt { get; set; }
    }
}
