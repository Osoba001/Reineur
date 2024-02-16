namespace Users.Infrastucture.Mails.PasswordResetMail
{
    internal class PasswordResetPayload
    {
        public required string Receiver { get; set; }
        public required int RecoveryPin { get; set; }
    }
}
