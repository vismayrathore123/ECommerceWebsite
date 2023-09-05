
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
//using System.Net.Mail;
using MailKit.Net.Smtp;


namespace WebsiteSolution.CommonHelper
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            MimeMessage toemail = new MimeMessage();
            toemail.From.Add(MailboxAddress.Parse("MyGmailId"));
            toemail.To.Add(MailboxAddress.Parse(email));
            toemail.Subject = subject;
            toemail.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };
            using (var emailClient = new SmtpClient())
            {
                emailClient.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                emailClient.Authenticate("vismayrathore2021@gmail.com", "nerbrugqmkzzxklx");
                emailClient.Send(toemail);
                emailClient.Disconnect(true);

            }

            return Task.CompletedTask; ;
        }
    }
}
