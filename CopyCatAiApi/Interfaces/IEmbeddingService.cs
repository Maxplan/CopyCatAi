using CopyCatAiApi.Models;

namespace CopyCatAiApi.Interfaces
{
    public interface IEmbeddingService
    {
        Task SaveTextEmbeddingAsync(TextEmbeddingModel textEmbedding);
        Task SaveEmbeddingsForTextBlocksAsync(List<string> textBlocks, int conversationId, string userId);
        Task<List<TextEmbeddingModel>> GetEmbeddingsByConversationIdAsync(int conversationId);
    }
}