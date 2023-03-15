using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Startup
{
    public static class OtherFeatures
    {
        public static void ListenActivity()
        {
            var activityListener = new ActivityListener();
            activityListener.ShouldListenTo = (activitySource) => activitySource.Name == Tracing.ActivitySourceName;
            activityListener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData;
            activityListener.ActivityStarted += (e) =>
            {

            };
            activityListener.ActivityStopped += (e) =>
            {

            };
            ActivitySource.AddActivityListener(activityListener);
        }

        public static void GenerateCertificateRA()
        {
            var ra = KeyGenerator.GenerateRootAuthority();
            var certData = ra.Export(X509ContentType.Pfx, "Password");
            File.WriteAllBytes(@"c:\Projects\SimpleIdServer\certificates\simpleIdServer.pfx", certData);
        }

        public static void CreateCertificateFromRA()
        {
            var parentCertificate = new X509Certificate2(@"c:\Projects\SimpleIdServer\certificates\simpleIdServer.pfx", "Password");
            var selfSignedCertificate = KeyGenerator.GenerateSelfSignedCertificate(parentCertificate, "firstMtlsClient");
            var securityKey = new X509SecurityKey(selfSignedCertificate);
            PemConverter.ConvertFromSecurityKey(securityKey);
        }

        public static void CreateClientJWK()
        {
            var ecdsaSig = ClientKeyGenerator.GenerateECDsaSignatureKey("keyId", SecurityAlgorithms.EcdsaSha256);
            var pem = PemConverter.ConvertFromSecurityKey(ecdsaSig.Key);
            var jwk = JsonWebKeyConverter.ConvertFromSecurityKey(ecdsaSig.Key);
            var str = JsonNode.Parse(JsonExtensions.SerializeToJson(jwk)).AsObject();
            string ss = "";
        }
    }
}
