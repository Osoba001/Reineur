namespace Reineur.Share.EmailService
{
    public class EmailConfiguration
    {
        public required string Sender { get; set; }
        public required string Password { get; set; }
        public required string Host { get; set; }
        public required int Port { get; set; } = 587;
    }
}
