using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CopyCatAiApi.Models
{
    public class OpenAIResponse
    {
        public List<ChatChoice>? Choices { get; set; }
    }
}