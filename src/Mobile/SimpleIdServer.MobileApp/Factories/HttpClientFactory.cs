using System.Net.Http;

namespace SimpleIdServer.MobileApp.Factories
{
    public static class HttpClientFactory
    {
        public static HttpClient Build()
        {
            return new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                {
                    return true;
                }
            });
        }
    }
}
