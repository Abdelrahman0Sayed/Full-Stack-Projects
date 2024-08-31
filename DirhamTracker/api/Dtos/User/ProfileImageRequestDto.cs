namespace api.Dtos.User
{
    public class ProfileImageRequestDto
    {
        public string? Base64String { get; set; }
        public string? FileName { get; set; }
        public string? ContentType { get; set; }
        public string? Email { get; set; }
    }
}
