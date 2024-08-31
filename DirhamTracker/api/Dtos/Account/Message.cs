using MimeKit;

namespace api.Dtos.Account
{
    public class Message
    {
        public List<MailboxAddress> To { get; set; } = null!;
        public string? Subject { get; set; }
        public string? Content { get; set; }

        public Message(IEnumerable<string> to, string subject , string content)
        {
            To = new List<MailboxAddress>();
            To.AddRange(to.Select(x => new MailboxAddress("email", x)));

            Subject = subject;
            Content = content;
        }
    }
}
