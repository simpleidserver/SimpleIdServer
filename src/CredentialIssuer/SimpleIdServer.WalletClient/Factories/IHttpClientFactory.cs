using Microsoft.Extensions.Options;
using System.Net.Http;

namespace SimpleIdServer.WalletClient.Factories
{
    public interface IHttpClientFactory
    {
        HttpClient Build();
    }

    public class HttpClientFactory : IHttpClientFactory
    {
        private readonly WalletClientOptions _options;

        public HttpClientFactory(IOptions<WalletClientOptions> options)
        {
            _options = options.Value;
        }

        public HttpClient Build()
        {
            var handler = new HttpClientHandler();
            if (_options.IgnoreHttps) handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                return true;
            };
            return new HttpClient(handler);
        }
    }
}
