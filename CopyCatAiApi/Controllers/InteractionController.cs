using System.Security.Claims;
using CopyCatAiApi.Data.Contexts;
using CopyCatAiApi.DTOs;
using CopyCatAiApi.Models;
using CopyCatAiApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.IO;

namespace CopyCatAiApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class InteractionController : ControllerBase
    {
        private readonly OpenAIService _openAIService;
        private readonly UserManager<UserModel> _userManager;
        private readonly ConversationService _conversationService;
        private readonly FileService _fileService;
        private readonly MongoDbContext _dbContext;
        private readonly EmbeddingService _embeddingService;
        private readonly SimilaritySearchService _similaritySearchService;

        public InteractionController(SimilaritySearchService similaritySearchService, EmbeddingService embeddingService, OpenAIService openAIService, UserManager<UserModel> userManager, ConversationService conversationService, FileService fileService, MongoDbContext mongoDbContext)
        {
            _openAIService = openAIService;
            _userManager = userManager;
            _conversationService = conversationService;
            _fileService = fileService;
            _dbContext = mongoDbContext;
            _embeddingService = embeddingService;
            _similaritySearchService = similaritySearchService;
        }

        [HttpPost("Sendmessage")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequestModel request)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            if (!request.ConversationId.HasValue)
            {
                var newConversation = new ConversationModel()
                {
                    UserId = userId,
                    Timestamp = DateTime.Now,
                };

                await _conversationService.SaveConversationToDatabase(newConversation);
                request.ConversationId = newConversation.ConversationId;
            }

            var lastRequest = request.Conversation!.Last().Content!;

            var requestModel = new RequestModel()
            {
                Request = lastRequest,
                ConversationId = request.ConversationId.Value,
                TimeStamp = DateTime.Now
            };

            var responseContent = await _openAIService.SendMessageToOpenAI(request.Conversation!);

            var responseModel = new ResponseModel()
            {
                Response = responseContent,
                ConversationId = request.ConversationId.Value,
                TimeStamp = DateTime.Now
            };

            await _conversationService.SaveRequestToDatabase(requestModel);
            await _conversationService.SaveResponseToDatabase(responseModel);

            return Ok(new { Response = responseContent, conversationId = request.ConversationId });
        }


        [HttpPost("StartConversation")]
        public async Task<IActionResult> StartConversation()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            var newConversation = new ConversationModel()
            {
                UserId = userId,
                Timestamp = DateTime.Now
            };

            await _conversationService.SaveConversationToDatabase(newConversation);

            return Ok(new { conversationId = newConversation.ConversationId });
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

        [HttpPost("ProcessPdfAndSaveEmbeddings")]
        public async Task<IActionResult> ProcessPdfAndSaveEmbeddings(int conversationId, string userId)
        {
            var FilePath = "./Data/TestFiles/test2.pdf";
            using var stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            var textBlocks = _fileService.ConvertPdfToText(stream);
            //if (pdfFile == null || pdfFile.Length == 0)
            //    return BadRequest("No file provided.");
            //
            //using var stream = pdfFile.OpenReadStream();
            //var textBlocks = _fileService.ConvertPdfToText(stream);

            await _embeddingService.SaveEmbeddingsForTextBlocksAsync(textBlocks, conversationId, userId);

            return Ok("PDF processed and embeddings saved.");
        }
        [HttpGet("GetEmbeddingsByConversationId")]
        public async Task<IActionResult> GetEmbeddingsByConversationId(int conversationId)
        {
            var embeddings = await _embeddingService.GetEmbeddingsByConversationIdAsync(conversationId);

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
