using Microsoft.OpenApi.Services;
using CopyCatAiApi.Models;
using CopyCatAiApi.Interfaces;

namespace CopyCatAiApi.Services
{
    public class SimilaritySearchService
    {
        private readonly EmbeddingServiceFactory _embeddingServiceFactory;
        private readonly IServiceProvider _serviceProvider;

        public SimilaritySearchService(EmbeddingServiceFactory embeddingServiceFactory, IServiceProvider serviceProvider)
        {
            _embeddingServiceFactory = embeddingServiceFactory;
            _serviceProvider = serviceProvider;
        }

        public async Task<List<Models.SearchResult>> PerformSimilaritySearch(string prompt, int conversationId, double threshold)
        {
            var embeddingService = _embeddingServiceFactory.Create();
            var openAIService = _serviceProvider.GetRequiredService<OpenAIService>();
            var promptEmbedding = await openAIService.GetEmbedding(prompt);
            var embeddings = await embeddingService.GetEmbeddingsByConversationIdAsync(conversationId);

            var results = new List<Models.SearchResult>();

            foreach (var embedding in embeddings)
            {
                var score = CosineSimilarity(promptEmbedding, embedding.Embedding!);
                if (score >= threshold)
                {
                    results.Add(new Models.SearchResult
                    {
                        BlockId = embedding.BlockId,
                        Text = embedding.Text,
                        SimilarityScore = score
                    });
                }
            }

            // Sort by descending similarity score
            results.Sort((a, b) => b.SimilarityScore.CompareTo(a.SimilarityScore));

            // Take only the top 5 results
            return results.Take(10).ToList();
        }

        // Get the most similar text block
        public double CosineSimilarity(List<float> vectorA, List<float> vectorB)
        {
            var dotProduct = 0.0;
            var normA = 0.0;
            var normB = 0.0;

            for (var i = 0; i < vectorA.Count; i++)
            {
                dotProduct += vectorA[i] * vectorB[i];
                normA += Math.Pow(vectorA[i], 2);
                normB += Math.Pow(vectorB[i], 2);
            }

            return dotProduct / (Math.Sqrt(normA) * Math.Sqrt(normB));
        }
    }
}