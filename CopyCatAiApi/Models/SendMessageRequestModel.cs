using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CopyCatAiApi.Services.OpenAIService;

namespace CopyCatAiApi.Models
{
    public class SendMessageRequestModel
    {
        public List<ChatMessage>? Conversation { get; set; }
        public int? ConversationId { get; set; }
    }
}