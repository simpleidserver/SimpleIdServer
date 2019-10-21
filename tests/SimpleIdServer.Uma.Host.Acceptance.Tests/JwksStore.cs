using SimpleIdServer.Jwt;
using System.Security.Cryptography;

namespace SimpleIdServer.Uma.Host.Acceptance.Tests
{
    public class JwksStore
    {
        private static object _obj = new object();
        private JsonWebKey _jsonWebKey;
        private static JwksStore _instance;

        private JwksStore()
        {
            using (var rsa = RSA.Create())
            {
                _jsonWebKey = new JsonWebKeyBuilder().NewSign("1", new[]
                {
                    KeyOperations.Sign,
                    KeyOperations.Verify
                }).SetAlg(rsa, "RS256").Build();
            }
        }

        public static JwksStore GetInstance()
        {
            lock(_obj)
            {
                if (_instance == null)
                {
                    _instance = new JwksStore();
                }

                return _instance;
            }
        }

        public JsonWebKey GetJsonWebKey()
        {
            return _jsonWebKey;
        }
    }
}
