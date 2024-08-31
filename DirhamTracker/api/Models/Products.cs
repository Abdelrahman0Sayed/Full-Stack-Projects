using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

namespace api.Models;

public class Products
{
    [Required]
    [MaxLength(150)]
    public int Id { get; set; }

    
    public string? ProductIdnetifier { get; set; }
    
    [MaxLength(30)]
    public string? Category { get; set; }
    
    [MinLength(16)]
    [MaxLength(16)]
    public string? CardNumber { get; set; }
    
    public float Paid { get; set; }
    
    public DateTime? PaymentTime { get; set; } = DateTime.Now;

    
    public string? AppUserId { get; set; }
    public ApplicationUser? AppUser { get; set; }
}