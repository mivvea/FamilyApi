using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FamilyApi.DataModels
{
    public class Movie
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("title")]
        public string? Title { get; set; }

        [BsonElement("poster")]
        public string? Poster { get; set; }

        [BsonElement("addedBy")]
        public string? AddedBy { get; set; }
    }
}
