using System.Security.Claims;
using CopyCatAiApi.Models;
using CopyCatAiApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static CopyCatAiApi.Services.OpenAIService;

namespace CopyCatAiApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class InteractionController : ControllerBase
    {
        private readonly OpenAIService _openAIService;
        private readonly UserManager<UserModel> _userManager;
        private readonly ConversationService _conversationService;

        public InteractionController(OpenAIService openAIService, UserManager<UserModel> userManager, ConversationService conversationService)
        {
            _openAIService = openAIService;
            _userManager = userManager;
            _conversationService = conversationService;
        }

        [HttpPost("Sendmessage")]
        public async Task<IActionResult> SendMessage([FromBody] List<ChatMessage> conversation, int? conversationId = null)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            if (!conversationId.HasValue)
            {
                var newConversation = new ConversationModel()
                {
                    UserId = userId,
                    Timestamp = DateTime.Now
                };

                await _conversationService.SaveConversationToDatabase(newConversation);
                conversationId = newConversation.ConversationId;
            }

            var request = new RequestModel()
            {
                Request = conversation.Last().Content!,
                ConversationId = conversationId.Value,
                TimeStamp = DateTime.Now
            };

            var responseContent = await _openAIService.SendMessageToOpenAI(conversation);

            var response = new ResponseModel()
            {
                Response = responseContent,
                ConversationId = conversationId.Value,
                TimeStamp = DateTime.Now
            };

            await _conversationService.SaveRequestToDatabase(request);
            await _conversationService.SaveResponseToDatabase(response);


            return Ok(new { Response = responseContent, conversationId });
        }
    }
}
