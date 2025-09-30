
using Backend.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Backend.Utility
{
    public class EmailSender : IEmailSender<AppUser>
    {
        public Task SendConfirmationLinkAsync(AppUser user, string email, string confirmationLink)
        {
            Console.WriteLine($"Sending confirmation link to {email}: {confirmationLink}");
            return Task.CompletedTask;
        }

        public Task SendPasswordResetLinkAsync(AppUser user, string email, string resetLink)
        {
            Console.WriteLine($"Sending password reset link to {email}: {resetLink}");
            return Task.CompletedTask;
        }

        public Task SendPasswordResetCodeAsync(AppUser user, string email, string resetCode)
        {
            Console.WriteLine($"Sending password reset code to {email}: {resetCode}");
            return Task.CompletedTask;
        }
    }
}
