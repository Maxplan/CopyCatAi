// Purpose: Model for similarity search results.

namespace CopyCatAiApi.Models
{
    public class SearchResult
    {
        public string? BlockId { get; set; }
        public string? Text { get; set; }
        public double SimilarityScore { get; set; }
    }
}