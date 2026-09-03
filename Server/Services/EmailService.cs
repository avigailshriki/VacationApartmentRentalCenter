using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendWelcomeEmail(string toEmail, string userName)
        {
            var smtpHost = _configuration["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
            var smtpPort = int.TryParse(_configuration["EmailSettings:SmtpPort"], out var port) ? port : 587;
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderDisplayEmail = _configuration["EmailSettings:SenderDisplayEmail"] ?? "VacationApartments@co.il";
            var appPassword = _configuration["EmailSettings:AppPassword"];

            if (string.IsNullOrWhiteSpace(senderEmail) || string.IsNullOrWhiteSpace(appPassword))
            {
                // הגדרות המייל לא הוגדרו (למשל בסביבת פיתוח חדשה) - יש להגדיר אותן ב-User Secrets ולא כאן בקוד.
                throw new InvalidOperationException(
                    "הגדרות שליחת מייל (EmailSettings:SenderEmail / EmailSettings:AppPassword) לא הוגדרו. יש להגדירן ב-User Secrets של פרויקט WebAPI.");
            }

            var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(senderEmail, appPassword),
                EnableSsl = true
            };
            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderDisplayEmail),
                Subject = "ברוך הבא ל-Vacation Apartments!",
                Body = $"<h1>שלום {userName}!</h1><p>איזה כיף שנרשמת למערכת שלנו.</p>",
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }
    }
}
