using CopyCatAiApi.Models;
using MongoDB.Driver;

namespace CopyCatAiApi.Data.Contexts
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database = null!;
        public MongoDbContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetValue<string>("CosmosDb:ConnectionString"));
            if (client != null)
                _database = client.GetDatabase(configuration.GetValue<string>("CosmosDb:DatabaseName"));
        }

        public IMongoCollection<TextEmbeddingModel> TextEmbeddings => _database.GetCollection<TextEmbeddingModel>("TextEmbeddings");
    }
}