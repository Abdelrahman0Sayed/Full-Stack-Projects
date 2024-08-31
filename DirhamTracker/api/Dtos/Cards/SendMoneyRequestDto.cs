using Microsoft.Identity.Client;

namespace api.Dtos.Cards
{
    public class SendMoneyRequestDto
    {
        public float   Amount { get; set; }
        public string? SourceCardNumber { get; set; }
        public string? DestCardNumber { get; set; }
        public string? Email { get; set; }
    }
}
