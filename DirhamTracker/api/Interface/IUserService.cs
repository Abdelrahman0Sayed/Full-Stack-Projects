using api.Dtos.User;
using api.Models;

namespace api.Interface
{
    public interface IUserService
    {
        public Task<UserResponseDto> GetUser(string email);
        public Task<UserResponseDto> EditProfile(UserRequestDto updatedUser);
        public Task<string> SetProfileImageAsync(IFormFile? newImage , string Email);
    }
}
