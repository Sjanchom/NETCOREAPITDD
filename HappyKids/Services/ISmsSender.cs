using System.Threading.Tasks;

namespace HappyKids.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
