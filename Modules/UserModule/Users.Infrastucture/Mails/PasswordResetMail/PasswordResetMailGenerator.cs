using Reineur.Share.EmailService;
using System.Reflection;

namespace Users.Infrastucture.Mails.PasswordResetMail
{
    internal class PasswordResetMailGenerator : IMailGenerator<PasswordResetPayload>
    {
        public PasswordResetMailGenerator(IMailSender emailSender, DeploymentConfiguration deploymentConfig)
        {
            _emailSender = emailSender;
            DeploymentConfiguration = deploymentConfig;
            _htmlString = GetHtmlString();
        }

        private string GetHtmlString()
        {
            string htmlString = "";
            var executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var pathToHtmlFile = Path.Combine(executableLocation, @"Mails/PasswordResetMail/PasswordResetMailBody.html");
            if (File.Exists(pathToHtmlFile))
            {
                htmlString = File.ReadAllText(pathToHtmlFile);
                htmlString = htmlString.Replace("[SupportTeamEmail]", DeploymentConfiguration.SupportTeamEmail);
            }
            return htmlString;
        }

        private void ReplacePasswordResetUrl(PasswordResetPayload payload)
        {
            string PasswordResetUrl = $"{DeploymentConfiguration.PasswordRecoveryPageUrl}?pin={payload.RecoveryPin}&email={payload.RecoveryPin}";
            _htmlString = _htmlString.Replace("[PasswordResetUrl]", PasswordResetUrl);

        }


        public async Task<ActionResponse> SendAsync(PasswordResetPayload payLoad)
        {
            if (!string.IsNullOrEmpty(_htmlString))
            {
                ReplacePasswordResetUrl(payLoad);
                var email = new EmailDto { To = new List<string> { payLoad.Receiver }, Body = _htmlString, Subject = "Swinva Passsword Recovery." };
                return await _emailSender.SendEmailAsync(new List<EmailDto> { email });
            }
            return BadRequestResult("Html String is empty");
        }

        private readonly DeploymentConfiguration DeploymentConfiguration;
        private readonly IMailSender _emailSender;
        private string _htmlString;
    }
}
