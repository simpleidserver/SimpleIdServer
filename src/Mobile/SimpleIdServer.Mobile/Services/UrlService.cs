using Microsoft.Extensions.Options;

namespace SimpleIdServer.Mobile.Services
{
    public interface IUrlService
    {
        string GetUrl(string url);
    }

    public class UrlService : IUrlService
    {
        private readonly MobileOptions _options;

        public UrlService(IOptions<MobileOptions> options)
        {
            _options = options.Value;
        }

        public string GetUrl(string url) => _options.IsDev ? url.Replace("loc alhost", MobileOptions.LocalhostIp) : url;
    }
}