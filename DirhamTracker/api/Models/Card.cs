using api.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class Card 
    {
        [Required]
        [MaxLength(16)]
        [MinLength(16)]
        public string CardNumber { get; set; }

        [MaxLength(4)]
        public string? CVVCode { get; set; }

        public string? ExpiresDate { get; set; }

        public float TotalBalance { get; set; }

        public string? AppUserId { get; set; }

        public ApplicationUser? AppUser { get; set; }
    }
}
