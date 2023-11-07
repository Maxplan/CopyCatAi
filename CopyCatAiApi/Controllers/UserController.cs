using Microsoft.AspNetCore.Mvc;

namespace CopyCatAiApi.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        // simple test that returns 1
        [HttpGet]
        [Route("test")]
        public int Test()
        {
            return 1;
        }
    }
}