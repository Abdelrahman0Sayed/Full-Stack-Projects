namespace api.Dtos.Cards
{
    public class CardResponseDto
    {
        public string? Message { get; set; }
        
        public string? CardNumber { get; set; }
        
        public string? CVVCode { get; set; }

        public string? ExpiresOn { get; set; } 

        public float TotalBalance { get; set; }

    }
}
