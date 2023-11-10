// Purpose: Service for communicating with OpenAI API.

using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;

namespace CopyCatAiApi.Services
{
    public class OpenAIService
    {
        // 
        private readonly HttpClient _httpClient;

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

        public async Task<string> SendMessageToOpenAI(string message)
        {
            var data = new
            {
                messages = new[]
                {
                    new { role = "user", content = message }
        },
                model = "gpt-3.5-turbo"
            };

            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("chat/completions", content);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<OpenAIResponse>(responseString);


                return responseObject.Choices?[0].Message?.Content ?? "";
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new Exception($"OpenAI API call failed: {response.StatusCode}, Details: {errorResponse}");
            }
        }

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
