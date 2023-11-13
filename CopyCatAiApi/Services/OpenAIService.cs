// Purpose: Service for communicating with OpenAI API.

using System.Text;
using Newtonsoft.Json;

namespace CopyCatAiApi.Services
{
    public class OpenAIService
    {
        // Private fields
        private readonly HttpClient _httpClient;

        // Constructor using dependency injection
        public OpenAIService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://api.openai.com/v1/");

            var apiKey = configuration["OpenAI:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("OpenAI API key is missing.");
            }

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        }
        // Send a message to OpenAI
        public async Task<string> SendMessageToOpenAI(List<ChatMessage> conversation)
        {
            // Create the data object
            var data = new
            {
                messages = conversation.Select(m => new { role = m.Role, content = m.Content }).ToArray(),
                model = "gpt-3.5-turbo"
            };

            // Create the content
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            // Send the message
            var response = await _httpClient.PostAsync("chat/completions", content);

            // Check if the response was successful
            if (response.IsSuccessStatusCode)
            {
                // Read the response
                var responseString = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<OpenAIResponse>(responseString);

                // Return the response
                return responseObject.Choices?[0].Message?.Content ?? "";
            }
            else
            {
                // Read the error response
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new Exception($"OpenAI API call failed: {response.StatusCode}, Details: {errorResponse}");
            }
        }

        // Classes for deserializing the response from OpenAI
        public class OpenAIResponse
        {
            public List<ChatChoice>? Choices { get; set; }
        }

        public class ChatChoice
        {
            public ChatMessage? Message { get; set; }
        }

        public class ChatMessage
        {
            public string? Role { get; set; }
            public string? Content { get; set; }
        }
    }
}
