using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FamilyApi.DataModels
{
    public class Movie : BaseItem
    {
        [BsonElement("title")]
        public string? Title { get; set; }

        [BsonElement("poster")]
        public string? Poster { get; set; }
    }
}
