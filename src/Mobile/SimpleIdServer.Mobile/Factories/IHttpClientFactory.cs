using Microsoft.Extensions.Options;

namespace SimpleIdServer.Mobile.Factories;

public interface IHttpClientFactory
{
    HttpClient Build();
}

public class HttpClientFactory : IHttpClientFactory
{
    private readonly MobileOptions _options;

    public HttpClientFactory(IOptions<MobileOptions> options)
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
