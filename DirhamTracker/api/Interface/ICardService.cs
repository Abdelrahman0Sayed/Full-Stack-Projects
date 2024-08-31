using api.Dtos.Cards;
using api.Models;
using System.Data.SqlTypes;

namespace api.Interface
{
    public interface ICardService
    {
        public Task<CardResponseDto> AddCardAsync(CardRequestDto newCard);
        
        public Task<List<CardResponseDto>> ViewCardsAsync(string userEmail);

        public Task<string> SendMoney(SendMoneyRequestDto money);

        public Task<bool> GetMoney(SendMoneyRequestDto destinatedCard);

        public Task<string> DeleteCard(string cardNumber);
    }
}
