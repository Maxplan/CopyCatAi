// Purpose: Model for Request data.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CopyCatAiApi.Models
{
    public class RequestModel
    {
        [Key]
        public int RequestId { get; set; } // Primary key for the request table
        public string Request { get; set; } = ""; // The request from the user
        public DateTime TimeStamp { get; set; } // The time the request was sent
        public string? PreferenceProfile { get; set; } // The preference profile of the user
        public string Model { get; set; } = "gpt-3.5-turbo"; // The model of the user

        [ForeignKey("ConversationId")]
        public int ConversationId { get; set; } // Foreign key for the conversation table
        public ConversationModel? Conversation { get; set; } // Navigation property for the conversation table

    }
}