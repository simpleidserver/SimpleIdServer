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

    public UriProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
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
