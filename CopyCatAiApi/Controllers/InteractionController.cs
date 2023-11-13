using System.Text;
using CopyCatAiApi.Services;
using Microsoft.AspNetCore.Mvc;
using static CopyCatAiApi.Services.OpenAIService;

namespace CopyCatAiApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class InteractionController : ControllerBase
    {
        private readonly OpenAIService _openAIService;

        public InteractionController(OpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        [HttpPost("Sendmessage")]
        public async Task<IActionResult> SendMessage([FromBody] List<ChatMessage> conversation)
        {
            var response = await _openAIService.SendMessageToOpenAI(conversation);
            return Ok(response);
        }
    }
}