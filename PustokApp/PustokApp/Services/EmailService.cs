using System.Net;
using System.Net.Mail;
using PustokApp.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var emailSettings = _configuration.GetSection("EmailSettings");

        var host = emailSettings["Host"];
        var port = int.Parse(emailSettings["Port"]);
        var email = emailSettings["Email"];
        var password = emailSettings["Password"];

        using var client = new SmtpClient(host, port)
        {
            
            Credentials = new NetworkCredential(email, password),
            EnableSsl = true
        };

        var message = new MailMessage
        {
            From = new MailAddress(email),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        message.To.Add(toEmail);

        await client.SendMailAsync(message);
    }
}