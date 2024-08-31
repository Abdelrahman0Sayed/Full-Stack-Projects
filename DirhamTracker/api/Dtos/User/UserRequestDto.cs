using System.Diagnostics.Contracts;

namespace api.Dtos.User
{
    public class UserRequestDto
    {
        public string? CustomerName { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Bio { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? DOB { get; set; }
        public string? ProfileImage { get; set; }

    }
}
