using Reineur.Share.EmailService;

namespace Reineur.Share
{
    public class EnvVariable
    {
        public static readonly string USER_DB_CONNECT_STRING = Environment.GetEnvironmentVariable("USER_DB_CONNECT_STRING") ?? "Data Source=.;Initial Catalog=ReineurUserDb;Integrated Security=True;Trust Server Certificate=True";
        public static readonly string ORIGIN = Environment.GetEnvironmentVariable("ORIGIN") ?? "http://localhost:3000";

        public static readonly string AUTH_SECRET_KEY = Environment.GetEnvironmentVariable("AUTH_SECRET_KEY") ?? "e5l848r64w,krruiewh9hgu94gjuu2.ttvte468d";

        public static readonly TimeSpan ACCESS_TOKEN_TTL = AccessTokenLifeSpan();

        public static readonly EmailConfiguration EMAIL_CONFIGURATION = new()
        {
            Host = Environment.GetEnvironmentVariable("EMAIL_HOST") ?? "smtp.ethereal.email",
            Port = int.Parse(Environment.GetEnvironmentVariable("EMAIL_PORT") ?? "587"),
            Password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD") ?? "r4E5MXfZysGWf4pqya",
            Sender = Environment.GetEnvironmentVariable("EMAIL_SENDER") ?? "zackary.oconnell@ethereal.email"
        };
        public static readonly DeploymentConfiguration DEPLOYMENT_CONFIGURATION = new()
        {
            LogoUrl = Environment.GetEnvironmentVariable("LOGO_URL") ?? "",
            PasswordRecoveryPageUrl = Environment.GetEnvironmentVariable("PASSWORD_RECOVERY_PAGE_URL") ?? "",
            SupportTeamEmail = Environment.GetEnvironmentVariable("SUPPORT_TEAM_EMAIL") ?? "supportteam@reineur.com",
            SupperAdminEmail = Environment.GetEnvironmentVariable("SUPPER_ADMIN_EMAIL") ?? "admin@reineur.com"
        };

        private static TimeSpan AccessTokenLifeSpan()
        {
            var res = Environment.GetEnvironmentVariable("ACCESS_TOKEN_TTL");
            if (res == null)
                return TimeSpan.FromMinutes(60);
            return TimeSpan.FromMinutes(double.Parse(res));
        }
    }
}
