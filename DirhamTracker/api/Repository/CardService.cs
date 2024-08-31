using System.Runtime.InteropServices.JavaScript;
using api.Data;
using api.Dtos.Cards;
using api.Interface;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace api.Repository
{
    public class CardService : ICardService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public CardService(UserManager<ApplicationUser> userManager , ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }


        public async Task<CardResponseDto> AddCardAsync(CardRequestDto newCard)
        {
            // Get the user
            var user = await _userManager.FindByEmailAsync(newCard.Email);
            if (user is null)
                return new CardResponseDto { Message = "User isn't exist" };

            // Make sure the card isn't exist
            var card = _context.Card.FirstOrDefault(x => x.CardNumber == newCard.CardNumber);
            if (card is not null) {
                return new CardResponseDto { Message = "The Card Number is already used." };
            }

            card = new Card
            {
                CardNumber = newCard.CardNumber,
                CVVCode = newCard.CVVCode,
                ExpiresDate = newCard.ExpiresDate,
                TotalBalance = 0,
                AppUserId = user?.Id
            };
            await _context.Card.AddAsync(card);
            await _context.SaveChangesAsync();

            return new CardResponseDto
            {
                Message = "" ,
                CardNumber = newCard.CardNumber,
                CVVCode = newCard.CVVCode,
                ExpiresOn = newCard.ExpiresDate
            };
        }

        public async Task<List<CardResponseDto>> ViewCardsAsync(string userEmail)
        {
            //Get the userId of this email
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user is null)
                return null;

            var cards = await _context.Card.Where(x => x.AppUserId == user.Id).Select(x => new CardResponseDto
            {
                Message = "" ,
                CardNumber = x.CardNumber,
                CVVCode = x.CVVCode,
                ExpiresOn = x.ExpiresDate,
                TotalBalance = x.TotalBalance

            }).ToListAsync();
            return cards;
        }


        public async Task<string> SendMoney(SendMoneyRequestDto money)
        {
            // Get the user.
            var user = await _userManager.FindByEmailAsync(money.Email);
            if (user is null)
                return "User isn't exist.";
            
            // Validate the amount
            if (money.Amount == 0 || money.Amount.GetType() != typeof(float))
                return "Please Enter the amount of money";

            if (money.DestCardNumber == "")
                return "Please enter a card number";

            if (money.SourceCardNumber == "")
                return "Please Choose a Card";
            
            // Get the card of this user
            var card = _context.Card.Where(x => x.CardNumber == money.SourceCardNumber && x.AppUserId == user.Id).FirstOrDefault();
            if (card is null)
                return "Card isn't exist.";

            if (money.SourceCardNumber == money.DestCardNumber)
                return "You can't send money to yourself";
            
            if (card.TotalBalance < money.Amount)
                return "You don't have enough money in this card";

            card.TotalBalance -= money.Amount;


            // Send the Money
            var result = await GetMoney(money);
            if (!result)
                return "Card Isn't Exist";

            return "";
        }


        public async Task<bool> GetMoney(SendMoneyRequestDto destinatedCard)
        {
            var card = _context.Card.Where(x => x.CardNumber == destinatedCard.DestCardNumber).FirstOrDefault();
            if (card is null)
                return false;

            card.TotalBalance += destinatedCard.Amount;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<string> DeleteCard(string cardNumber)
        {
            // Make sure that this card exist.
            var card = await _context.Card.FindAsync(cardNumber);
            if (card is null)
                return "Card Isn't Exist!!";

            _context.Card.Remove(card);
            await _context.SaveChangesAsync();

            return "";
        }
    }
}
