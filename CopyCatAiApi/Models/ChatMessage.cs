using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CopyCatAiApi.Models
{
    public class ChatMessage
    {
        public string? Role { get; set; }
        public string? Content { get; set; }
    }
}