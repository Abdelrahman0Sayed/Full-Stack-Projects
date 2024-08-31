using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace api.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string CustomerName {get; set;} = String.Empty;
        //PhoneNumber

        [MaxLength(50)]
        public string? Bio { get; set; }

        [MaxLength(30)]
        public string? Country { get; set; }

        [MaxLength(20)]
        public string? City { get; set; }

        [MaxLength(200)]
        public string? ProfileImage { get; set; }

        public DateTime? DOB { get; set; }

        public string? OTP {get; set;}
        
        public List<Card>? Cards { get; set; }
        public List<Products> ProductsList { get; set; }
       
    }
}