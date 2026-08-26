using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Services
{
    public class EmailService
    {
        public async Task SendWelcomeEmail(string toEmail, string userName)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("a0583217526@gmail.com", "zppyonmscpseymuh"),
                EnableSsl = true
            };
            var mailMessage = new MailMessage
            {
                From = new MailAddress("VacationApartments@co.il"),
                Subject = "ברוך הבא ל-Vacation Apartments!",
                Body = $"<h1>שלום {userName}!</h1><p>איזה כיף שנרשמת למערכת שלנו.</p>",
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }
    }
}