// Purpose: Model for OpenAI response.

namespace CopyCatAiApi.Models
{
    public class OpenAIResponse
    {
        public List<ChatChoice>? Choices { get; set; }
    }
}