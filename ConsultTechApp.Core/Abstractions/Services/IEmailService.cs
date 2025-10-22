namespace ConsultTechApp.Core.Abstractions.Services;

public interface IEmailService
{
    public Task SendEmailAsync(string to, string subject, string body);
}
