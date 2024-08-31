using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Account
{
    public class AccountResponseDto
    {
        public string? Message{get; set;}
        public string? Email{get; set;}
        public string? UserName {get; set;}
        public bool IsAuthenticated {get; set;} = false;
        public string? Token{get;set;}
        public DateTime ExpiresOn {get; set;} = DateTime.Now.AddMinutes(10);
    }
}