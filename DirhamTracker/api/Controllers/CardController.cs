using api.Data;
using api.Dtos.Cards;
using api.Interface;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CardController : ControllerBase
    {
        private readonly ICardService _cardService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CardController(ICardService cardService, ApplicationDbContext context, UserManager<ApplicationUser> userManage)
        {
            _cardService = cardService;
            _context = context;
            _userManager = userManage;
        }

        [HttpPost("AddCard")]
        public async Task<IActionResult> AddCardAsync([FromBody] CardRequestDto newCard)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _cardService.AddCardAsync(newCard);

            if (response.Message != "")
                return BadRequest(response);



            return Ok(response);

        }
    
        [HttpGet("ViewCards")]
        public async Task<IActionResult> ViewCardsAsync(string userEmail)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _cardService.ViewCardsAsync(userEmail);
            if (response is null)
                return NotFound();

            return Ok(response);

        }
    
        [HttpPost("SendMoney")]
        public async Task<IActionResult> SendMoneyAsync(SendMoneyRequestDto money)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _cardService.SendMoney(money);
            if (response != "")
                return BadRequest(response);

            return Ok(response);
        }

        [HttpDelete("DeleteCard")]
        public async Task<IActionResult> DeleteCard(string cardNumber)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _cardService.DeleteCard(cardNumber);
            if (response != "")
                return BadRequest(response);

            return Ok(response);
        } 
    }
}
