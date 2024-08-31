using api.Models;
using System.ComponentModel.DataAnnotations;
namespace api.Dtos.User
{
    public class UserResponseDto
    {
        public string CustomerName { get; set; } = String.Empty;

        public string? UserName { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Bio { get; set; }

        public string? Country { get; set; }

        public string? City { get; set; }

        public string? DOB { get; set; }

        public string? ProfileImage { get; set; }


    }
}
