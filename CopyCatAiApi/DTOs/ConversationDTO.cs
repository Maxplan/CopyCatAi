using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CopyCatAiApi.DTOs
{
    public class ConversationDTO
    {
        public int ConversationId { get; set; }
        public IList<string> Requests { get; set; } = new List<string>();
        public IList<string> Responses { get; set; } = new List<string>();
        public DateTime Timestamp { get; set; }
    }
}