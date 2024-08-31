using api.Data;
using api.Dtos.User;
using api.Interface;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting; // Ensure you have this using directive
using System.IO;
using System.Threading.Tasks;

namespace api.Repository
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        
        public UserService(UserManager<ApplicationUser> userManager, ApplicationDbContext context , IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _context = context;
            _environment = environment;
        }

        public async Task<UserResponseDto> GetUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return null;

            return new UserResponseDto
            {
                CustomerName = user.CustomerName,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Bio = user.Bio,
                Country = user.Country,
                City = user.City,
                DOB = user.DOB.HasValue ? user.DOB.Value.ToString("MM/dd/yyyy") : string.Empty
            };
        }

        public async Task<UserResponseDto> EditProfile(UserRequestDto updatedUser)
        {
            var user = await _userManager.FindByEmailAsync(updatedUser.Email);
            if (user is null)
                return null;

            user.CustomerName = updatedUser.CustomerName;
            user.PhoneNumber = updatedUser.PhoneNumber;
            user.Bio = updatedUser.Bio;
            user.Country = updatedUser.Country;
            user.City = updatedUser.City;
            user.DOB = DateTime.Parse(updatedUser.DOB);

            await _context.SaveChangesAsync();

            return new UserResponseDto
            {
                CustomerName = updatedUser.CustomerName,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = updatedUser.PhoneNumber,
                Bio = updatedUser.Bio,
                Country = updatedUser.Country,
                City = updatedUser.City,
                DOB = user.DOB.HasValue ? user.DOB.Value.ToString("MM/dd/yyyy") : string.Empty
            };
        }

        public async Task<string> SetProfileImageAsync(IFormFile? newImage , string Email)
        {
            if (newImage == null || newImage.Length == 0)
                return "";

            var user = await _userManager.FindByEmailAsync(Email);
            if (user is null)
                return "";
            
            /*var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest("Invalid   
                file format");
            }*/
            
            var uploadsFolder = Path.Combine(_environment.WebRootPath ,"uploads" );
            
            if (!Directory.Exists(uploadsFolder))
            {
                try
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                catch
                {
                    throw new IOException("Can't Crate Uploads Folder");
                }
            }
            var imagePath = Path.Combine(uploadsFolder, newImage.FileName);
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await newImage.CopyToAsync(stream);
            }

            var image = $"https://localhost:7045/uploads/{newImage.FileName}";
            user.ProfileImage = image;
            await _context.SaveChangesAsync();
            
            return image;
        }
    }
}