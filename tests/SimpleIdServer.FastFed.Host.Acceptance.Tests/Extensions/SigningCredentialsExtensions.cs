using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace SimpleIdServer.FastFed.Host.Acceptance.Tests.Extensions
{
    public static class SigningCredentialsExtensions
    {
        public static JsonWebKey SerializePublicJWK(this SigningCredentials credentials, string use = "sig")
        {
            if (credentials.Key is RsaSecurityKey rsa)
            {
                var parameters = rsa.Rsa.ExportParameters(false);
                credentials = new SigningCredentials(new RsaSecurityKey(parameters) { KeyId = credentials.Kid }, credentials.Algorithm);
            }

            if (credentials.Key is ECDsaSecurityKey ecdsa)
            {
                var parameters = ecdsa.ECDsa.ExportParameters(false);
                credentials = new SigningCredentials(new ECDsaSecurityKey(ECDsa.Create(parameters)) { KeyId = credentials.Kid }, credentials.Algorithm);
            }

            var result = SerializeJWK(credentials, use);
            return result;
        }

        public static JsonWebKey SerializeJWK(this SigningCredentials credentials, string use = "sig")
        {
            var result = JsonWebKeyConverter.ConvertFromSecurityKey(credentials.Key);
            if (!string.IsNullOrWhiteSpace(use)) result.Use = use;
            result.Alg = credentials.Algorithm;
            if (credentials.Key is X509SecurityKey)
            {
                var securityKey = credentials.Key as X509SecurityKey;
                var rsaKey = securityKey.Certificate.PublicKey.GetRSAPublicKey();
                if (rsaKey != null)
                {
                    var parameters = rsaKey.ExportParameters(false);
                    result.E = Base64UrlEncoder.Encode(parameters.Exponent);
                    result.N = Base64UrlEncoder.Encode(parameters.Modulus);
                }
            }

            return result;
        }
    }
}
