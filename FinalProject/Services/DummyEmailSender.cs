using Microsoft.AspNetCore.Identity.UI.Services;
namespace FinalProject.Services
{
    public class DummyEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            Console.WriteLine($"Email: {email}, Subject: {subject}, Body: {htmlMessage}");
            return Task.CompletedTask;
        }
    }
}
