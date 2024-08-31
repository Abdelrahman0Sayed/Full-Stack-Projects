using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Account;
using api.Models;

namespace api.Services
{
    public interface IAuthServices
    {
        public string CreateToken(ApplicationUser user);
        
        public Task<AccountResponseDto> Register(RegisterModel newUser);
       
        public Task<AccountResponseDto> LoginAsync(LoginModel user);
        
        public Task SendEmailAsync(string mailTo , string subject , string message , IList<IFormFile> attachments);
        
    }
}