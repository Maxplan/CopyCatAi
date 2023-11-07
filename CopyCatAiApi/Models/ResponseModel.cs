// Purpose: Model for Response data.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CopyCatAiApi.Models
{
    public class ResponseModel
    {
        [Key]
        public int ResponseId { get; set; } // Primary key for the response table
        public string Response { get; set; } = ""; // The response from the AI
        public DateTime TimeStamp { get; set; } = DateTime.Now; // The time the response was sent

        //Navigation Properties
        [ForeignKey("ConversationId")]
        public int ConversationId { get; set; } // Foreign key for the conversation table
        public ConversationModel Conversation { get; set; } = new ConversationModel(); // Navigation property for the conversation table
    }
}