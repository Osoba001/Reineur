using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using Reineur.Share;
using static Reineur.Share.ActionResponse;

namespace Reineur.Share.EmailService
{
    internal class EmailKitService(EmailConfiguration optConfig) : IMailSender
    {
        private readonly EmailConfiguration _config = optConfig;

        public async Task<ActionResponse> SendEmailAsync(List<EmailDto> emails)
        {
            var messages = new List<MimeMessage>();
            foreach (var email in emails)
            {
                messages.Add(CreateEmailMessage(email));
            }

            using var smtp = new SmtpClient();
            smtp.Connect(_config.Host, _config.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_config.Sender, _config.Password);
            string errorMessage = string.Empty;
            try
            {
                var sendTask = messages.Select(emails => smtp.SendAsync(emails));
                await Task.WhenAll(sendTask);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
            if (errorMessage != string.Empty)
                return BadRequestResult(errorMessage);
            return SuccessResult();
        }
        private MimeMessage CreateEmailMessage(EmailDto command)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config.Sender));
            foreach (var receiver in command.To)
            {
                email.To.Add(MailboxAddress.Parse(receiver));
            }
            email.Subject = command.Subject;
            email.Body = new TextPart(TextFormat.Html) { Text = command.Body };
            return email;
        }

    }
}
