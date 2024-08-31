using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class OTP
    {
        public int Id {get; set;}

        [Required]
        [MaxLength(4)]
        public int TokenNumber {get;set;}
        public DateTime ExpiresOn {get; set;} = DateTime.Now.AddMinutes(10); 

        public string? UserId {get;set;}
        public ApplicationUser? AppUser {get;set;}
    }
}