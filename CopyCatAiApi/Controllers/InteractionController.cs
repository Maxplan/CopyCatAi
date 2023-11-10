using System.Text;
using CopyCatAiApi.Services;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> SendMessage([FromBody] string message)
        {
            var response = await _openAIService.SendMessageToOpenAI(message);
            return Ok(response);
        }
    }
}