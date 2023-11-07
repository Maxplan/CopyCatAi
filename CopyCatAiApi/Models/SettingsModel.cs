// Purpose: Model for user settings
namespace CopyCatAiApi.Models
{
    public class SettingsModel
    {
        public ThemeEnum Theme { get; set; } = ThemeEnum.Light; // Set to local machine default
        public string language { get; set; } = "en-US"; // Set to local machine default
        public string PreferenceProfile { get; set; } = ""; // Used for sending specific preferences to Ai Model
    }
}