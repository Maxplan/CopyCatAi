// Purpose: Controller for interaction with Frontend

using System.Security.Claims;
using CopyCatAiApi.Data.Contexts;
using CopyCatAiApi.DTOs;
using CopyCatAiApi.Interfaces;
using CopyCatAiApi.Models;
using CopyCatAiApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.IO;
using Org.BouncyCastle.Ocsp;
using static CopyCatAiApi.Services.OpenAIService;

namespace CopyCatAiApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class InteractionController : ControllerBase
    {
        // --Dependency Injection--
        private readonly OpenAIService _openAIService;
        private readonly UserManager<UserModel> _userManager;
        private readonly ConversationService _conversationService;
        private readonly FileService _fileService;
        private readonly MongoDbContext _dbContext;
        private readonly EmbeddingServiceFactory _embeddingServiceFactory;
        private readonly SimilaritySearchService _similaritySearchService;

        // --Constructor--
        public InteractionController(SimilaritySearchService similaritySearchService, EmbeddingServiceFactory embeddingServiceFactory, OpenAIService openAIService, UserManager<UserModel> userManager, ConversationService conversationService, FileService fileService, MongoDbContext mongoDbContext)
        {
            _openAIService = openAIService;
            _userManager = userManager;
            _conversationService = conversationService;
            _fileService = fileService;
            _dbContext = mongoDbContext;
            _embeddingServiceFactory = embeddingServiceFactory;
            _similaritySearchService = similaritySearchService;
        }

        // --Methods--
        // Send a message to OpenAI
        [HttpPost("Sendmessage")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequestModel request)
        {
            // Get the user ID from the JWT token
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            // If the user ID is null, return an unauthorized response
            if (userId == null)
            {
                return Unauthorized();
            }
            // If the conversation ID is null, create a new conversation
            if (!request.ConversationId.HasValue)
            {
                var newConversation = new ConversationModel()
                {
                    UserId = userId,
                    Timestamp = DateTime.Now,
                };
                // Save the conversation to the database
                await _conversationService.SaveConversationToDatabase(newConversation);
                // Set the conversation ID to the new conversation's ID
                request.ConversationId = newConversation.ConversationId;
            }

            // Send the message to OpenAI
            // Get the last message in the conversation
            var lastRequest = request.Conversation!.Last().Content!;
            // Send the last message to OpenAI
            var requestModel = new RequestModel()
            {
                Request = lastRequest,
                ConversationId = request.ConversationId.Value,
                TimeStamp = DateTime.Now
            };
            // Get the response from OpenAI
            var responseContent = await _openAIService.SendMessageToOpenAI(request.Conversation!);
            // Save the request and response to the database
            var responseModel = new ResponseModel()
            {
                Response = responseContent,
                ConversationId = request.ConversationId.Value,
                TimeStamp = DateTime.Now
            };

            await _conversationService.SaveRequestToDatabase(requestModel);
            await _conversationService.SaveResponseToDatabase(responseModel);
            // Return the response from OpenAI
            return Ok(new { Response = responseContent, conversationId = request.ConversationId });
        }

        // Send a message to OpenAI with a PDF file
        // Send a message to OpenAI with a PDF file
        [HttpPost("SendPdfMessage")]
        public async Task<IActionResult> SendPdfMessage([FromForm] SendMessageRequestModel request, [FromForm] IFormFile pdfFile)
        {
            // Get the user ID from the JWT token
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            // If the conversation ID is null, create a new conversation
            if (!request.ConversationId.HasValue)
            {
                var newConversation = new ConversationModel()
                {
                    UserId = userId,
                    Timestamp = DateTime.Now,
                };
                // Save the conversation to the database
                await _conversationService.SaveConversationToDatabase(newConversation);
                // Set the conversation ID to the new conversation's ID
                request.ConversationId = newConversation.ConversationId;
            }

            // Check if conversation has at least one message
            if (request.Conversation == null || request.Conversation.Count == 0)
            {
                return BadRequest("The conversation cannot be empty.");
            }

            // Extract the last user message from the conversation
            var lastUserMessage = request.Conversation.LastOrDefault(m => m.Role == "user")?.Content;
            if (string.IsNullOrEmpty(lastUserMessage))
            {
                return BadRequest("No user message found in the conversation.");
            }

            // Process PDF and save embeddings
            using var stream = new MemoryStream();
            await pdfFile.CopyToAsync(stream);
            stream.Position = 0;
            var textBlocks = _fileService.ConvertPdfToText(stream);
            var embeddingService = _embeddingServiceFactory.Create();
            await embeddingService.SaveEmbeddingsForTextBlocksAsync(textBlocks, request.ConversationId.Value, userId);

            // Perform Similarity Search
            var similarTextBlocks = await _similaritySearchService.PerformSimilaritySearch(lastUserMessage, request.ConversationId.Value, 0.5);
            var concatenatedText = string.Join(" ", similarTextBlocks.Select(block => block.Text));

            // Send concatenated text and user prompt to OpenAI
            var openAIRequest = $"Prompt: {lastUserMessage} \n PDF: {concatenatedText}";
            var responseContent = await _openAIService.SendMessageToOpenAI(new List<ChatMessage> { new ChatMessage { Role = "user", Content = openAIRequest } });

            // Save Request and Response to Database
            var requestModel = new RequestModel()
            {
                Request = openAIRequest,
                ConversationId = request.ConversationId.Value,
                TimeStamp = DateTime.Now
            };

            var responseModel = new ResponseModel()
            {
                Response = responseContent,
                ConversationId = request.ConversationId.Value,
                TimeStamp = DateTime.Now
            };

            await _conversationService.SaveRequestToDatabase(requestModel);
            await _conversationService.SaveResponseToDatabase(responseModel);

            // Return the response from OpenAI
            return Ok(new { Response = responseContent, ConversationId = request.ConversationId });
        }




        // Start a new conversation
        [HttpPost("StartConversation")]
        public async Task<IActionResult> StartConversation()
        {
            // Get the user ID from the JWT token
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            // If the user ID is null, return an unauthorized response
            if (userId == null)
            {
                return Unauthorized();
            }
            // Create a new conversation
            var newConversation = new ConversationModel()
            {
                UserId = userId,
                Timestamp = DateTime.Now
            };
            // Save the conversation to the database
            await _conversationService.SaveConversationToDatabase(newConversation);
            // Return the conversation ID
            return Ok(new { conversationId = newConversation.ConversationId });
        }

        [HttpGet("GetConversationById")]
        public async Task<IActionResult> GetConversationById(int conversationId)
        {
            var conversation = await _conversationService.GetConversationById(conversationId);

            if (conversation == null)
            {
                return NotFound("Conversation not found.");
            }

            return Ok(conversation);
        }

        [HttpGet("GetConversationDetails")]

        public async Task<IActionResult> GetConversationDetails(int conversationId)
        {
            var conversation = await _conversationService.GetConversationById(conversationId);

            if (conversation == null)
            {
                return NotFound("Conversation not found.");
            }

            var requests = await _conversationService.GetRequestsByConversationId(conversationId);
            var responses = await _conversationService.GetResponsesByConversationId(conversationId);

            if (!requests.Any() && !responses.Any())
            {
                return NotFound("Data not found.");
            }


            var conversationDTO = new ConversationDTO()
            {
                ConversationId = conversation.ConversationId,
                Requests = requests.Select(r => r.Request).ToList(),
                Responses = responses.Select(r => r.Response).ToList(),
                Timestamp = requests.First().TimeStamp
            };

            return Ok(conversationDTO);
        }

        [HttpPost("RateResponse")]
        public async Task<IActionResult> RateResponse(int responseId, bool rating)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            var response = await _conversationService.GetResponseById(responseId);

            if (response == null)
            {
                return NotFound();
            }

            response.UserRating = rating;

            await _conversationService.SaveResponseToDatabase(response);

            return Ok();
        }

        [HttpGet("GetConversationByUserId")]
        public async Task<IActionResult> GetConversationByUserId()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            var conversations = await _conversationService.GetConversationsByUserId(userId);
            var conversationsReqRes = new List<ConversationDTO>();

            foreach (var convo in conversations)
            {
                var requests = await _conversationService.GetRequestsByConversationId(convo.ConversationId);
                var responses = await _conversationService.GetResponsesByConversationId(convo.ConversationId);

                if (!requests.Any())
                {
                    continue;
                }

                var DTO = new ConversationDTO()
                {
                    ConversationId = convo.ConversationId,
                    Requests = requests.Select(r => r.Request).ToList(),
                    Responses = responses.Select(r => r.Response).ToList(),
                    Timestamp = convo.Timestamp
                };
                conversationsReqRes.Add(DTO);
            }

            return Ok(conversationsReqRes);
        }

        [HttpDelete("DeleteConversation")]
        public async Task<IActionResult> DeleteConversation(int conversationId)
        {

            var conversation = await _conversationService.GetConversationById(conversationId);

            if (conversation == null)
            {
                return NotFound();
            }

            await _conversationService.DeleteConversationById(conversationId);

            return Ok("Conversation deleted.");
        }

        [HttpGet("GetEmbeddingsByConversationId")]
        public async Task<IActionResult> GetEmbeddingsByConversationId(int conversationId)
        {
            var embeddingService = _embeddingServiceFactory.Create();
            var embeddings = await embeddingService.GetEmbeddingsByConversationIdAsync(conversationId);

            return Ok(embeddings);
        }
        [HttpGet("get-prompt-embedding")]
        public async Task<IActionResult> GetPromptEmbedding(string prompt)
        {
            var embedding = await _openAIService.GetEmbedding(prompt);

            return Ok(embedding);
        }
        [HttpGet("get-similarity-search")]
        public async Task<IActionResult> GetSimilaritySearch(string prompt, int conversationId, double threshold)
        {
            var results = await _similaritySearchService.PerformSimilaritySearch(prompt, conversationId, threshold);

            return Ok(results);
        }
    }
}
