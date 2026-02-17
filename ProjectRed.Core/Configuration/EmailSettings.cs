namespace ProjectRed.Core.Configuration
{
    public class EmailSettings
    {
        public string SenderName { get; set; } = null!;
        public string SenderEmail { get; set; } = null!;
        public string SenderPassword { get; set; } = null!;
        public string MailServer { get; set; } = null!;
    }
}
