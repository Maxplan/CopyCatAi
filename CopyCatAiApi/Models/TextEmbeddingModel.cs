// Purpose: Model for Text Embedding data, used to push data to CosmosDb

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CopyCatAiApi.Models
{
    public class TextEmbeddingModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public int ConversationId { get; set; }
        public string? BlockId { get; set; }
        public string? Title { get; set; }
        public List<float>? Embedding { get; set; }
        public string? Text { get; set; }
        public long Timestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    }
}