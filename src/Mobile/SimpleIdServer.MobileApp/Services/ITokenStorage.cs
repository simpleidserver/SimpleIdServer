using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace SimpleIdServer.MobileApp.Services
{
    public interface ITokenStorage
    {
        Task Store(AuthInfo authInfo);
        Task<JwtSecurityToken> GetToken();
        Task<AuthInfo> GetAuthInfo();
        bool Remove();
    }
}
