// Purpose: Provide services for embedding data.

using CopyCatAiApi.Data.Contexts;
using CopyCatAiApi.Interfaces;
using CopyCatAiApi.Models;
using MongoDB.Driver;

namespace CopyCatAiApi.Services
{
    public class EmbeddingService : IEmbeddingService
    {
        private readonly MongoDbContext _dbContext;
        private readonly OpenAIService _openAIService;

        public EmbeddingService(MongoDbContext dbContext, OpenAIService openAIService)
        {
            _dbContext = dbContext;
            _openAIService = openAIService;
        }

        // Save a text embedding to the database
        public async Task SaveTextEmbeddingAsync(TextEmbeddingModel textEmbedding)
        {
            await _dbContext.TextEmbeddings.InsertOneAsync(textEmbedding);
        }
        // Save a list of text embeddings to the database
        public async Task SaveEmbeddingsForTextBlocksAsync(List<string> textBlocks, int conversationId, string userId)
        {
            // Get the embedding for each text block
            foreach (var block in textBlocks)
            {
                // Get the embedding
                var embedding = await _openAIService.GetEmbedding(block);
                // Create the text embedding model
                var textEmbeddingModel = new TextEmbeddingModel
                {
                    UserId = userId,
                    ConversationId = conversationId,
                    BlockId = $"{conversationId}-{textBlocks.IndexOf(block) + 1}",
                    Embedding = embedding,
                    Text = block
                };
                // Save the text embedding
                await SaveTextEmbeddingAsync(textEmbeddingModel);
            }
        }
        // Get all text embeddings for a conversation
        public async Task<List<TextEmbeddingModel>> GetEmbeddingsByConversationIdAsync(int conversationId)
        {
            //
            return await _dbContext.TextEmbeddings.Find(t => t.ConversationId == conversationId).ToListAsync();
        }
    }
}
