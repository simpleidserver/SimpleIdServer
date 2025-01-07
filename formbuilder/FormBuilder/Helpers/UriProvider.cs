using Microsoft.AspNetCore.Http;

namespace FormBuilder.Helpers;

public interface IUriProvider
{
    string GetActiveFormCssUrl(string id);
    string GetAbsoluteUriWithVirtualPath();
    string GetRelativePath();
}

public class UriProvider : IUriProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UriProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetActiveFormCssUrl(string id)
        => $"{GetAbsoluteUriWithVirtualPath()}/forms/{id}/styles/active";

    public string GetAbsoluteUriWithVirtualPath()
    {
        var requestMessage = _httpContextAccessor.HttpContext.Request;
        var host = requestMessage.Host.Value;
        var http = "http://";
        if (requestMessage.IsHttps)
        {
            http = "https://";
        }

        var relativePath = requestMessage.PathBase.Value;
        return http + host + relativePath;
    }
    public string GetRelativePath()
    {
        var requestMessage = _httpContextAccessor.HttpContext.Request;
        return requestMessage.PathBase.Value;
    }
}
