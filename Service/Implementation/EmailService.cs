using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implementation
{
    class EmailService : IEmailService
    {
        private readonly EmailSender _emailSender;

        public EmailService(EmailSender emailSender)
        {
            _emailSender = emailSender;
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

            await _emailSender.SendEmailAsync(email, subject, message);
        }
    }
}
