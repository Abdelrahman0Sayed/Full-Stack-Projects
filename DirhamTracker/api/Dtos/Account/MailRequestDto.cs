﻿using System.ComponentModel.DataAnnotations;

namespace api.Dtos.Account;

public class MailRequestDto
{
    [Required]
    public string? ToEmail { get; set; }
    
    [Required]
    public string? Subject { get; set; }
    
    [Required]
    public string? Body { get; set; }
    
    public IList<IFormFile>? Attachments { get; set; }
}