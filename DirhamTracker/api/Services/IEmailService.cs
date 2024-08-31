using api.Dtos.Account;

namespace api.Services
{
    public interface IEmailService
    {
        public Task SendEmailAsync(string mailTo , string subject , string message , IList<IFormFile> attachments);
    }
}
