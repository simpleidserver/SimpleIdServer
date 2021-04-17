using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace SimpleIdServer.MobileApp.Services
{
    public class TokenStorage : ITokenStorage
    {
        private const string STORAGE_KEY = "token";

        public Task Store(AuthInfo authInfo)
        {
            var json = JsonConvert.SerializeObject(authInfo);
            return SecureStorage.SetAsync(STORAGE_KEY, json);
        }

        public async Task<JwtSecurityToken> GetToken()
        {
            var authInfo = await GetAuthInfo();
            if (authInfo == null)
            {
                return null;
            }

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(authInfo.IdToken);
            if (DateTime.UtcNow > jsonToken.ValidTo)
            {
                Remove();
                return null;
            }

            return jsonToken;
        }

        public async Task<AuthInfo> GetAuthInfo()
        {
            var json = await SecureStorage.GetAsync(STORAGE_KEY);
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<AuthInfo>(json);
        }

        public bool Remove()
        {
            return SecureStorage.Remove(STORAGE_KEY);
        }
    }
}
