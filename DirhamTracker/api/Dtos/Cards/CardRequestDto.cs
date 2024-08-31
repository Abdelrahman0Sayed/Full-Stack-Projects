using api.Models;
using System.ComponentModel.DataAnnotations;

namespace api.Dtos.Cards
{
    public class CardRequestDto
    {
        [Required]
        [MaxLength(16)]
        public string? CardNumber { get; set; }

        [MaxLength(4)]
        public string? CVVCode { get; set; }

        public string? ExpiresDate { get; set; }

        public string? Email { get; set; }
    }
}
