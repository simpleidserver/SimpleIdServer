using System.Threading.Tasks;

namespace SimpleIdServer.MobileApp.Services
{
    public interface ILoginProvider
    {
        Task<AuthInfo> LoginAsync();
    }
}
