using FormBuilder.Models;
using FormBuilder.Stores;
using Microsoft.AspNetCore.Mvc;
using System.Text;

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

    [HttpGet("{templateId}/css")]
    public async Task<IActionResult> GetCss(string templateId, CancellationToken cancellationToken)
    {
        var form = await _templateStore.Get(templateId, cancellationToken);
        if (form == null) return new NoContentResult();
        return Content(TransformCss(form), "text/css");
    }

    [HttpGet("{templateId}/js")]
    public async Task<IActionResult> GetActiveJs(string templateId, CancellationToken cancellationToken)
    {
        var form = await _templateStore.Get(templateId, cancellationToken);
        if (form == null) return new NoContentResult();
        var js = await TransformJs(form);
        return Content(js, "application/javascript; charset=utf-8");
    }

    private static string TransformCss(Template template)
    {
        var strBuilder = new StringBuilder();
        foreach(var externalCss in template.Styles.Where(s => s.Category == TemplateStyleCategories.Lib))
        {
            strBuilder.AppendLine($"@import url('{externalCss.Value}');");
        }

        var customCss = template.Styles.SingleOrDefault(s => s.Category == TemplateStyleCategories.Custom);
        if (customCss != null) 
        {
            strBuilder.AppendLine(customCss.Value);
        }
        
        return strBuilder.ToString();
    }

    private async Task<string> TransformJs(Template template)
    {
        var strBuilder = new StringBuilder();
        using (var httpClient = _httpClientFactory.CreateClient("JsClient"))
        {
            foreach (var externalCss in template.Styles.Where(s => s.Category == TemplateStyleCategories.Lib))
            {
                var script = await httpClient.GetStringAsync(externalCss.Value);
                strBuilder.AppendLine(script);
            }
        }

        var customCss = template.Styles.SingleOrDefault(s => s.Category == TemplateStyleCategories.Custom);
        if (customCss != null)
        {
            strBuilder.AppendLine(customCss.Value);
        }

        return strBuilder.ToString();
    }
}