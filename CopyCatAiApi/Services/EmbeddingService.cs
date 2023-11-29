// Purpose: Provide services for embedding data.

using CopyCatAiApi.Data.Contexts;
using CopyCatAiApi.Models;
using MongoDB.Driver;

namespace CopyCatAiApi.Services
{
    public class EmbeddingService
    {
        private readonly MongoDbContext _dbContext;
        private readonly OpenAIService _openAIService;

        public EmbeddingService(MongoDbContext dbContext, OpenAIService openAIService)
        {
            _dbContext = dbContext;
            _openAIService = openAIService;
        }


        public async Task SaveTextEmbeddingAsync(TextEmbeddingModel textEmbedding)
        {
            await _dbContext.TextEmbeddings.InsertOneAsync(textEmbedding);
        }

        public async Task SaveEmbeddingsForTextBlocksAsync(List<string> textBlocks, int conversationId, string userId)
        {
            foreach (var block in textBlocks)
            {
                var embedding = await _openAIService.GetEmbedding(block);
                var textEmbeddingModel = new TextEmbeddingModel
                {
                    UserId = userId,
                    ConversationId = conversationId,
                    BlockId = $"{conversationId}-{textBlocks.IndexOf(block) + 1}",
                    Embedding = embedding,
                    Text = block
                };
                await SaveTextEmbeddingAsync(textEmbeddingModel);
            }
        }
        public async Task<List<TextEmbeddingModel>> GetEmbeddingsByConversationIdAsync(int conversationId)
        {
            return await _dbContext.TextEmbeddings.Find(t => t.ConversationId == conversationId).ToListAsync();
        }
    }
}
