using System.ComponentModel.DataAnnotations;

namespace api.Dtos.Account;

public class VerifyRequestDto
{
    [Required]
    public string? UserName { get; set; }
    
    [Required]
    public string? Email { get; set; }
    
    [Required]
    public string? VerificationLink { get; set; }
}