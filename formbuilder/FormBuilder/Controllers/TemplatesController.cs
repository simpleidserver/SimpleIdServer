using FormBuilder.Stores;
using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Controllers;

[Route("templates")]
public class TemplatesController : Controller
{
    private readonly ITemplateStore _templateStore;
    private readonly IHttpClientFactory _httpClientFactory;

    public TemplatesController(ITemplateStore templateStore, IHttpClientFactory httpClientFactory)
    {
        _templateStore = templateStore;
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet("{templateId}/css/{styleId}")]
    public async Task<IActionResult> GetCss(string templateId, string styleId, CancellationToken cancellationToken)
    {
        var form = await _templateStore.Get(templateId, cancellationToken);
        if (form == null)
        {
            return new NoContentResult();
        }

        var style = form.CssStyles.SingleOrDefault(s => s.Id == styleId);
        if (style == null)
        {
            return new NoContentResult();
        }

        return Content(style.Value, "text/css");
    }

    [HttpGet("{templateId}/js/{styleId}")]
    public async Task<IActionResult> GetJs(string templateId, string styleId, CancellationToken cancellationToken)
    {
        var form = await _templateStore.Get(templateId, cancellationToken);
        if (form == null)
        {
            return new NoContentResult();
        }

        var style = form.JsStyles.SingleOrDefault(s => s.Id == styleId);
        if (style == null)
        {
            return new NoContentResult();
        }

        return Content(style.Value, "application/javascript; charset=utf-8");
    }
}