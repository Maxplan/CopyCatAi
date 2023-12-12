// Purpose: Model for chat messages.

namespace CopyCatAiApi.Models
{
    public class ChatMessage
    {
        public string? Role { get; set; }
        public string? Content { get; set; }
    }
}