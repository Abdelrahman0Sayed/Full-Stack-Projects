using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using api.Data;
using api.Dtos;
using api.Dtos.Account;
using api.Models;
using api.Services;
using Azure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Engines;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        
        public AuthController(IAuthServices authService , UserManager<ApplicationUser> userManager , SignInManager<ApplicationUser> signInManager , ApplicationDbContext context){
            _authService = authService;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync(RegisterModel newUser){
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.Register(newUser);
            if (!result.IsAuthenticated)
                return BadRequest(result);

            // Generate Confirmation Url
            var user = await _userManager.FindByEmailAsync(newUser.Email);
            var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"http://localhost:3000/confirmEmail?token={confirmToken}&userEmail={user.Email}";


            var verify = new VerifyRequestDto()
            {
                UserName = user.UserName,
                Email = user.Email,
                VerificationLink = confirmationLink
            };
            VerifyEmail(verify);
            return Ok(result);
        }


        [HttpPost("VerifyEmail")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyRequestDto verifiedEmail)
        {
            var tempPath = $"{Directory.GetCurrentDirectory()}\\Templates\\Welcome.html";
            var mailText = await System.IO.File.ReadAllTextAsync(tempPath);

            // Replace placeholders with actual user data
            mailText = mailText.Replace("[username]", verifiedEmail.UserName)
                .Replace("[email]", verifiedEmail.Email)
                .Replace("[verify_link]", verifiedEmail.VerificationLink);

            await _authService.SendEmailAsync(verifiedEmail.Email, "Email Verification", mailText, null);
            return Ok();
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            string decodedToken = HttpUtility.UrlDecode(token);
            decodedToken = decodedToken.Replace(" ", "+");
            if (user != null)
            {
                var confirm = await _userManager.ConfirmEmailAsync(user, decodedToken);
                if (confirm.Succeeded)
                {
                    return StatusCode(StatusCodes.Status200OK);
                }
                
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }






        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(LoginModel user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _authService.LoginAsync(user);
            if (!response.IsAuthenticated)
                return BadRequest(response);
            
            // If the 2FA Enabled
            var appUser = await _userManager.FindByEmailAsync(user.Email);
            if (appUser.TwoFactorEnabled)
            {
                await _signInManager.SignOutAsync();
                await _signInManager.PasswordSignInAsync(appUser, user.Password ,  false, true);
                
                var otpToken = await _userManager.GenerateTwoFactorTokenAsync(appUser , "Email");
                appUser.OTP = otpToken;
                await _context.SaveChangesAsync();
                TwoFA(otpToken , appUser.Email , appUser.UserName);
            }
            
            return Ok(response);
        }
        // Second Step: Login With OTP
        [HttpPost("Login2FA")]
        public async Task<IActionResult> LoginOTP([FromBody] TwoFactorRequestDto requestDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(requestDto.Email);
                if (user == null)
                {
                    return BadRequest("User not found");
                }

                if(user.OTP == requestDto.Code)
                {
                    var token = _authService.CreateToken(user);
                    var result = new AccountResponseDto
                    {
                        Message = "",
                        Email = user.Email,
                        UserName = user.UserName,
                        IsAuthenticated = true,
                        Token = token,
                        ExpiresOn = DateTime.Now.AddMinutes(10)
                    };

                    user.OTP = "";
                    return Ok(result);
                }

                return BadRequest("Invalid OTP code");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        

        [HttpPost("SendEmail")]
        public async Task<IActionResult> SendMail([FromForm] MailRequestDto dto)
        {
            await _authService.SendEmailAsync(dto.ToEmail, dto.Subject, dto.Body, dto.Attachments);
            return Ok();
        }



        [HttpGet("2FA")]
        private async Task<IActionResult> TwoFA(string otpToken , string userEmail , string userName)
        {
            //Get 2FA Temp
            var tempPath = $"{Directory.GetCurrentDirectory()}\\Templates\\2FA.html";
            var mailText = await System.IO.File.ReadAllTextAsync(tempPath);

            mailText = mailText.Replace("[otp]", otpToken).Replace("[username]" , userName);
            // Lets Send the Email.
            await _authService.SendEmailAsync(userEmail , "OTP Verification" , mailText , null);

            return Ok();
        }
        




        // Reset Password
        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([Required] string userEmail)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user != null)
            {
                // Generate Reset Password Token 
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
 
                var resetLink = $"http://localhost:3000/ResetPassword?token={token}&userEmail={user.Email}";
                
                // Send the Email
                var tempPath = $"{Directory.GetCurrentDirectory()}\\Templates\\ForgotPassword.html";
                var mailText = await System.IO.File.ReadAllTextAsync(tempPath);

                // Replace placeholders with actual user data
                mailText = mailText.Replace("[username]", user.UserName)
                    .Replace("[email]", user.Email)
                    .Replace("[reset]", resetLink);

                await _authService.SendEmailAsync(user.Email, "Email Verification", mailText, null);
                return Ok();
            }

            return BadRequest("User isn't exist");
        }


        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequestDto resetPassword)
        {
            var user = await _userManager.FindByEmailAsync(resetPassword.Email);
            if (user != null)
            {
                var result =
                    await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.NewPassword);

                if (result.Succeeded)
                    return Ok("Password Reseted Sucessfully");
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code , error.Description);
                    }

                    return Ok(ModelState);
                }
            }

            return BadRequest("User is not found!");
        }
    }
}