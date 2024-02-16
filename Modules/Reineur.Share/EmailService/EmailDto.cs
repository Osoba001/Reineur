namespace Reineur.Share.EmailService
{
    public class EmailDto
    {
        public required List<string> To { get; set; }
        public required string Subject { get; set; }
        public required string Body { get; set; }
    }
}
