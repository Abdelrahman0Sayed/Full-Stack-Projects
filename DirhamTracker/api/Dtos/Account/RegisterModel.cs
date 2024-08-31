using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Account
{
    public class RegisterModel
    {
        [Required]
        [MaxLength(100)]
        public string? CustomerName {get; set;}
        
        [Required]
        [MaxLength(50)]
        public string? UserName {get; set;}

        [Required]
        [EmailAddress]
        public string? Email {get; set;}

        [Required]
        [DataType(DataType.Password)]
        public string? Password {get; set;}
    }
}