// Purpose: Service for communicating with OpenAI API.

using System.Text;
using CopyCatAiApi.Models;
using Newtonsoft.Json;

namespace CopyCatAiApi.Services
{
    public class OpenAIService
    {
        // Private fields
        private readonly HttpClient _httpClient;
        private readonly SimilaritySearchService _similarityService;

        // Constructor using dependency injection
        public OpenAIService(IHttpClientFactory httpClientFactory, IConfiguration configuration, SimilaritySearchService similaritySearchService)
        {
            _similarityService = similaritySearchService;
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://api.openai.com/v1/");
            // Get the API key from the configuration/user secrets
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
                var responseObject = JsonConvert.DeserializeObject<OpenAIResponse>(responseString)!;
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

        public async Task<string> SendPdfMessageToOpenAI(SendMessageRequestModel request)
        {
            // Perform the similarity search to get top 5 similar items
            var similarItems = await _similarityService.PerformSimilaritySearch(request.Conversation!.ToString()!, request.ConversationId!.Value, 0.5);

            // Extract the Text property from each SearchResult item and concatenate
            var concatenatedTexts = string.Join(" ", similarItems.Select(si => si.Text));

            // Construct the message to be sent to OpenAI
            string messageToSend = $"PDF File: {concatenatedTexts}\nPrompt: {request.Conversation!.Last().Content}";

            // Convert the message into ChatMessage format
            var chatMessage = new ChatMessage { Role = "user", Content = messageToSend };

            // Send the message to OpenAI
            return await SendMessageToOpenAI(new List<ChatMessage> { chatMessage });
        }


        // Get the embedding for a text
        public async Task<List<float>> GetEmbedding(string textBlock)
        {
            // Create the data object
            var data = new
            {
                model = "text-embedding-ada-002",
                input = textBlock
            };
            // Create the content
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            // Use the already configured _httpClient instance
            var response = await _httpClient.PostAsync("embeddings", content);
            // Check if the response was successful
            if (response.IsSuccessStatusCode)
            {
                // Read the response
                var responseContent = await response.Content.ReadAsStringAsync();
                // Deserialize the response
                var embeddingResponse = JsonConvert.DeserializeObject<EmbeddingResponse>(responseContent)!;
                // Return the embedding
                return embeddingResponse.Data?[0].Embedding!;
            }
            throw new Exception($"OpenAI API call failed: {response.StatusCode}");
        }

        // Classes for deserializing the response from OpenAI
    }
}
