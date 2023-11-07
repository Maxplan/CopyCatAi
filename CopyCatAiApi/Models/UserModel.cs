// Purpose: Model for the user table in the database. Contains all the information for a user.

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace CopyCatAiApi.Models
{
    public class UserModel : IdentityUser // Inherits from IdentityUser so we can use the built in authentication
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string FullName => $"{FirstName} {LastName}";

        public IEnumerable<ConversationModel> ConversationList { get; set; } = new List<ConversationModel>(); // List of conversations the user has been in


        // Navigation Properties
        [ForeignKey("SettingId")]
        public int SettingsId { get; set; } // Foreign key for the settings table
        public SettingsModel Settings { get; set; } = new SettingsModel(); // Navigation property for the settings table
    }
}