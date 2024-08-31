using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using api.Data;
using api.Dtos.Account;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using MailKit.Security;
using Microsoft.Extensions.Options;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace api.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        private readonly SignInManager<ApplicationUser> _signinManager;
        private readonly MailSettings _mailSettings;
        private readonly ApplicationDbContext _context;
        
        public AuthServices(UserManager<ApplicationUser> userManager, IConfiguration config, SignInManager<ApplicationUser> signinManager , IOptions<MailSettings> mailSettings , ApplicationDbContext context) {
            _userManager = userManager;
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));
            _signinManager = signinManager;
            _mailSettings = mailSettings.Value;
            _context = context;
        }

        public  string CreateToken(ApplicationUser user){
            // Add Claims - Info
            var claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.Email , user.Email ),
                new Claim(JwtRegisteredClaimNames.GivenName , user.UserName ),
            };

            // Signing Creds
            var creds = new SigningCredentials(_key , SecurityAlgorithms.HmacSha512Signature);

            // Descriptor
            var tokenDescriptor = new SecurityTokenDescriptor{
                Subject =new ClaimsIdentity(claims),
                SigningCredentials = creds,
                Expires = DateTime.Now.AddMinutes(10),
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"]
            };

            // Handler
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public async Task<AccountResponseDto> Register(RegisterModel newUser)
        {
            //Some Validation
            var userEmail = await _userManager.FindByEmailAsync(newUser.Email);
            if (userEmail is not null)
                return new AccountResponseDto{Message = "Email is already exist"};
            
            var userName = await _userManager.FindByNameAsync(newUser.UserName);
            if (userName is not null)
                return new AccountResponseDto{Message = "UserName is already taken"};

            //No we can create the user.
            var user = new ApplicationUser{
                CustomerName = newUser.CustomerName,
                UserName = newUser.UserName,
                Email = newUser.Email,
                TwoFactorEnabled = true,
            };

            var created = await _userManager.CreateAsync(user , newUser.Password);
            if (!created.Succeeded){
                var errors = String.Empty;

                foreach(var error in created.Errors){
                    errors += $"{error.Description}, ";
                }

                return new AccountResponseDto{Message= $"{errors}"};
            }

            // Add Role
            var result = await _userManager.AddToRoleAsync(user , "User");
            // كدا هو اتضاف لل داتا بيز ... نبدأ بقى نجهز الريسبونس
            var token = CreateToken(user);

            await _context.SaveChangesAsync();
            
            var response = new AccountResponseDto{
                Message = String.Empty,
                Email = user.Email,
                UserName = user.UserName,
                IsAuthenticated = true,
                Token = token,
                ExpiresOn = DateTime.Now.AddMinutes(10)
            };

            return response;

        }


        public async Task<AccountResponseDto> LoginAsync(LoginModel user)
        {
            // Some validation 
            var appUser =await  _userManager.FindByEmailAsync(user.Email);

            if (appUser is null )
                return new AccountResponseDto { Message = "Invalid Username or/and Password" };

            var passwordCheck = await _signinManager.CheckPasswordSignInAsync(appUser, user.Password, false);
            if (!passwordCheck.Succeeded)
                return new AccountResponseDto { Message = "Invalid Username or/and Password" };

            var token = CreateToken(appUser);
            var result = new AccountResponseDto
            {
                Message = "",
                Email = appUser.Email,
                UserName = appUser.UserName,
                IsAuthenticated = true,
                Token = token,
                ExpiresOn = DateTime.Now.AddMinutes(10)
            };

            return result;
        }
        
        
        public async Task SendEmailAsync(string mailTo, string subject, string body, IList<IFormFile> attachments)
        {
            // Prepare the email
            var email = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_mailSettings.Email),
                Subject = subject
            };

            email.To.Add(MailboxAddress.Parse(mailTo));

            // Prepare the attachments of email
            var builder = new BodyBuilder();
            if (attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in attachments)
                {
                    if (file.Length > 0)
                    {
                        using var ms = new MemoryStream();
                        file.CopyTo(ms);
                        fileBytes = ms.ToArray();

                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }

            // Prepare the body
            builder.HtmlBody = body;
            email.Body = builder.ToMessageBody();
            email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Email));
            
            // Prepare the email provider
            using var smtp = new SmtpClient();
            
            // Provider Settings
            smtp.Connect(_mailSettings.Host , _mailSettings.Port , SecureSocketOptions.StartTls);
            // Add Authentications
            smtp.Authenticate(_mailSettings.Email , _mailSettings.Password);

            await smtp.SendAsync(email);
            
            smtp.Disconnect(true);
        }

    }
}