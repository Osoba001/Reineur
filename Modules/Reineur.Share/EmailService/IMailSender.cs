using Reineur.Share;

namespace Reineur.Share.EmailService
{
    public interface IMailSender
    {
        Task<ActionResponse> SendEmailAsync(List<EmailDto> emails);
    }
}
