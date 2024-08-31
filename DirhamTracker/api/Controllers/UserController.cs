using api.Data;
using api.Dtos.User;
using api.Helpers;
using api.Interface;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        
        public UserController(IUserService userService ,IWebHostEnvironment environment ,UserManager<ApplicationUser> userManager , ApplicationDbContext context)
        {
            _userService = userService;
            _environment = environment;
            _userManager = userManager;
            _context = context;
        }

        
        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUserAsync(string email)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _userService.GetUser(email);
            if (response is null)
                return BadRequest("User isn't exist");
            return Ok(response);
        }

        
        
        [HttpPut("EditProfile")]
        public async Task<IActionResult> EditProfileAsync([FromBody] UserRequestDto updatedUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _userService.EditProfile(updatedUser);
            if (response is null)
                return NotFound();

            return Ok(response);
        }

        
        
        [HttpPost("SetProfileImage")]
        public async Task<IActionResult> SetProfileImage([FromBody] ProfileImageRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var imageFile = FileHelper.ConvertBase64ToIFormFile(request.Base64String, request.FileName, request.ContentType);

            var response = await _userService.SetProfileImageAsync(imageFile, request.Email);
            if (string.IsNullOrEmpty(response))
                return BadRequest("Something went wrong");

            return Ok(response);
        }



        [HttpDelete("DeleteProfileImage")]
        public async Task<IActionResult> DeleteProfileImage([FromBody] DeleteProfileImageDto  image)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(image.Email);
            if (user is null)
                return NotFound();

            user.ProfileImage = "";
            await _context.SaveChangesAsync();
            
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            var imagePath = Path.Combine(uploadsFolder, image.ImageName);
            if (System.IO.File.Exists(imagePath))
            {
                try
                {
                    System.IO.File.Delete(imagePath);
                } catch
                {
                    return BadRequest("Failed to delete the image.");
                }
            }
            else
            {
                return NotFound();
            }

            return Ok();
        }

        
        [HttpGet("GetProfileImage")]
        public async Task<IActionResult> GetImage(string Email)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(Email);
            if (user is null)
                return NotFound();

            return Ok(user.ProfileImage);
        }
    }
}
