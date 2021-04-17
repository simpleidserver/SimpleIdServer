using System.Threading.Tasks;

namespace SimpleIdServer.MobileApp.Services
{
    public interface IMessageService
    {
        Task Show(string title, string message);
        Task<bool> Show(string title, string message, string accept, string cancel);
    }
}
