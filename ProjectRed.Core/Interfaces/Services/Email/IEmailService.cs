namespace ProjectRed.Core.Interfaces.Services.Email
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email);
    }
}
