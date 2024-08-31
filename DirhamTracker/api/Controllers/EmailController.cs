using api.Dtos.Account;
using api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IAuthServices _authService;
        private readonly IEmailService _emailService;

        public EmailController(IAuthServices authService , IEmailService emailService){

            _authService = authService;
            _emailService = emailService;
        }

        [HttpGet("TestEmail")]
        public async Task<IActionResult> TestEmail()
        {
            // Create a Test Message
            var message = new Message(new string[] { "abdelrahmansayed880@gmail.com" }, "Bank Of Boulaq", "<h1>Test Email Verification</h1>");

            _emailService.SendEmailAsync(message);

            return Ok();
        }
    }
}
