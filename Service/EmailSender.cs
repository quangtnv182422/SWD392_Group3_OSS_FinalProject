using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var smtpServer = _configuration["EmailSettings:SmtpServer"];
        var smtpPortString = _configuration["EmailSettings:SmtpPort"];
        var smtpUsername = _configuration["EmailSettings:SmtpUsername"];
        var smtpPassword = _configuration["EmailSettings:SmtpPassword"];

        if (smtpServer == null || smtpPortString == null || smtpUsername == null || smtpPassword == null)
        {
            throw new InvalidOperationException("Email settings are not configured properly.");
        }

        var smtpPort = int.Parse(smtpPortString);

        using (var client = new SmtpClient(smtpServer, smtpPort))
        {
            client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
            client.EnableSsl = true;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpUsername),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);
        }
    }

    public async Task SendWelcomeEmail(string email, string username, string password)
    {
        string subject = "Welcome to Our Platform!";
        string message = $@"
                <h3>Hello {username},</h3>
                <p>Your account has been successfully created.</p>
                <p><strong>Username:</strong> {username}</p>
                <p><strong>Password:</strong> {password}</p>
                <p>Please change your password after logging in.</p>
                <p>Best regards,</p>
                <p>Your Company Name</p>
            
        ";

        await SendEmailAsync(email, subject, message);
    }
}
