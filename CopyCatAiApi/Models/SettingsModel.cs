// Purpose: Model for user settings
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CopyCatAiApi.Models
{
    public class SettingsModel
    {
        [Key]
        public int SettingsId { get; set; } // Primary key for the settings table
        public ThemeEnum Theme { get; set; } = ThemeEnum.Light; // Set to local machine default
        public string language { get; set; } = "en-US"; // Set to local machine default
        public string PreferenceProfile { get; set; } = ""; // Used for sending specific preferences to Ai Model

        [ForeignKey("User")]
        public string? UserId { get; set; }
        public UserModel? User { get; set; }
    }
}