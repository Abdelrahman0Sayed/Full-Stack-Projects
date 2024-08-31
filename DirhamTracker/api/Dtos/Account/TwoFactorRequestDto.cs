namespace api.Dtos.Account;

public class TwoFactorRequestDto
{
    public string? Email { get; set; }
    public string? Code { get; set; }
}