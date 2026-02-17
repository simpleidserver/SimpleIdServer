using FormBuilder.Models;
using Microsoft.AspNetCore.Http;

namespace FormBuilder.Helpers;

public interface IUriProvider
{
    string GetCssUrl(string templateId, TemplateStyle style);
    string GetJsUrl(string templateId, TemplateStyle style);
    string GetFormUrl(string id);
    string GetFormPublishUrl(string id);
    string GetWorkflowUrl(string id);
    string GetWorkflowPublishUrl(string id);
    string GetAbsoluteUriWithVirtualPath();
    string GetRelativePath();
}

public class UriProvider : IUriProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHttpRequestState _httpRequestState;

    public UriProvider(IHttpContextAccessor httpContextAccessor, IHttpRequestState httpRequestState)
    {
        _httpContextAccessor = httpContextAccessor;
        _httpRequestState = httpRequestState;
    }

    public string GetCssUrl(string templateId, TemplateStyle style)
    {
        if(style.Category == TemplateStyleCategories.Lib)
        {
            return style.Value;
        }

        return $"{GetAbsoluteUriWithVirtualPath()}/templates/{templateId}/css/{style.Id}";
    }

    public string GetJsUrl(string templateId, TemplateStyle style)
    {
        if (style.Category == TemplateStyleCategories.Lib)
        {
            return style.Value;
        }

        return $"{GetAbsoluteUriWithVirtualPath()}/templates/{templateId}/js/{style.Id}";
    }

    public string GetFormUrl(string id)
        => $"{GetAbsoluteUriWithVirtualPath()}/forms/{id}";

    public string GetFormPublishUrl(string id)
        => $"{GetAbsoluteUriWithVirtualPath()}/forms/{id}/publish";

    public string GetWorkflowUrl(string id)
        => $"{GetAbsoluteUriWithVirtualPath()}/workflows/{id}";

    public string GetWorkflowPublishUrl(string id)
        => $"{GetAbsoluteUriWithVirtualPath()}/workflows/{id}/publish";

    public virtual string GetAbsoluteUriWithVirtualPath()
    {
        if (_httpContextAccessor?.HttpContext != null)
        {
            var requestMessage = _httpContextAccessor.HttpContext.Request;
            var host = requestMessage.Host.Value;
            var http = requestMessage.IsHttps ? "https://" : "http://";
            var relativePath = requestMessage.PathBase.Value;
            return http + host + relativePath;
        }

        return _httpRequestState.GetAbsoluteUriWithVirtualPath();
    }
    public string GetRelativePath()
    {
        if (_httpContextAccessor?.HttpContext != null)
        {
            var requestMessage = _httpContextAccessor.HttpContext.Request;
            return requestMessage.PathBase.Value;
        }

        return _httpRequestState.PathBase ?? string.Empty;
    }
}
