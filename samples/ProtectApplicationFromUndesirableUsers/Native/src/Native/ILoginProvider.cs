using System.Threading.Tasks;

namespace Native
{
    public interface ILoginProvider
    {
        Task<AuthInfo> LoginAsync();
    }
}
