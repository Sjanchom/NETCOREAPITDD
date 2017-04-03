using System.Threading.Tasks;

namespace HappyKids.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
