using Reineur.Share;

namespace Reineur.Share.EmailService
{
    public interface IMailGenerator<TPayLoad>
    {
        Task<ActionResponse> SendAsync(TPayLoad payLoad);
    }
}
