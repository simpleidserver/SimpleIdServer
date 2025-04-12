using FormBuilder.Models;
using FormBuilder.Stores;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace FormBuilder.Controllers;

[Route("forms")]
public class FormsController : Controller
{
    private readonly IFormStore _formStore;
    private readonly IHttpClientFactory _httpClientFactory;

    public FormsController(IFormStore formStore, IHttpClientFactory httpClientFactory)
    {
        _formStore = formStore;
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet("{id}/{templateName}/css/active")]
    public async Task<IActionResult> GetActiveCss(string id, string templateName, CancellationToken cancellationToken)
    {
        var form = await _formStore.Get(id, cancellationToken);
        if (form == null) return new NoContentResult();
        return Content(TransformCss(form.GetActiveCssStyles(templateName)), "text/css");
    }

    [HttpGet("last/{correlationId}/{templateName}/css/active")]
    public async Task<IActionResult> GetLastActiveCss(string correlationId, string templateName, CancellationToken cancellationToken)
    {
        var form = await _formStore.GetLatestPublishedVersionByCorrelationId(correlationId, cancellationToken);
        if (form == null) return new NoContentResult();
        return Content(TransformCss(form.GetActiveCssStyles(templateName)), "text/css");
    }

    [HttpGet("{id}/{templateName}/js/active")]
    public async Task<IActionResult> GetActiveJs(string id, string templateName, CancellationToken cancellationToken)
    {
        var form = await _formStore.Get(id, cancellationToken);
        if (form == null) return new NoContentResult();
        var js = await TransformJs(form.GetActiveJsStyles(templateName));
        return Content(js, "application/javascript; charset=utf-8");
    }

    [HttpGet("last/{correlationId}/{templateName}/js/active")]
    public async Task<IActionResult> GetLastActiveStyle(string correlationId, string templateName, CancellationToken cancellationToken)
    {
        var form = await _formStore.GetLatestPublishedVersionByCorrelationId(correlationId, cancellationToken);
        if (form == null) return new NoContentResult();
        var js = await TransformJs(form.GetActiveJsStyles(templateName));
        return Content(js, "application/javascript; charset=utf-8");
    }

    private static string TransformCss(IEnumerable<FormStyle> formStyles)
    {
        var strBuilder = new StringBuilder();
        foreach(var externalCss in formStyles.Where(s => s.Category == FormStyleCategories.Lib))
        {
            strBuilder.AppendLine($"@import url('{externalCss.Value}');");
        }

        var customCss = formStyles.SingleOrDefault(s => s.Category == FormStyleCategories.Custom);
        if (customCss != null) 
        {
            strBuilder.AppendLine(customCss.Value);
        }
        
        return strBuilder.ToString();
    }

    private async Task<string> TransformJs(IEnumerable<FormStyle> formStyles)
    {
        var strBuilder = new StringBuilder();
        using (var httpClient = _httpClientFactory.CreateClient("JsClient"))
        {
            foreach (var externalCss in formStyles.Where(s => s.Category == FormStyleCategories.Lib))
            {
                var script = await httpClient.GetStringAsync(externalCss.Value);
                strBuilder.AppendLine(script);
            }
        }

        var customCss = formStyles.SingleOrDefault(s => s.Category == FormStyleCategories.Custom);
        if (customCss != null)
        {
            strBuilder.AppendLine(customCss.Value);
        }

        return strBuilder.ToString();
    }
}