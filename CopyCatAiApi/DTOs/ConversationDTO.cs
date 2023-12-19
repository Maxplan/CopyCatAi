using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CopyCatAiApi.Models;

namespace CopyCatAiApi.DTOs
{
    public class ConversationDTO
    {
        public int ConversationId { get; set; }
        public IList<string> Requests { get; set; } = new List<string>();
        public IList<ResponseModel> Responses { get; set; } = new List<ResponseModel>();
        public IList<string> RequestPrompts { get; set; } = new List<string>();
        public DateTime Timestamp { get; set; }
    }
}