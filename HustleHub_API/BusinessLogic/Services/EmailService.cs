using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class EmailService
{
    private readonly IConfiguration _config;
    public EmailService(IConfiguration config) => _config = config;

    public async Task SendEmailAsync(string subject, string body)
    {
        var settings = _config.GetSection("EmailSettings");
        var smtpClient = new SmtpClient(settings["SmtpServer"])
        {
            Port = int.Parse(settings["SmtpPort"]),
            Credentials = new NetworkCredential(settings["SmtpUser"], settings["SmtpPass"]),
            EnableSsl = true,
        };

        var mail = new MailMessage
        {
            From = new MailAddress(settings["From"]),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };
        mail.To.Add(settings["To"]);
        await smtpClient.SendMailAsync(mail);
    }
}