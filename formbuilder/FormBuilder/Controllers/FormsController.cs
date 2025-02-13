using FormBuilder.Stores;
using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Controllers;

[Route("forms")]
public class FormsController : Controller
{
    private readonly IFormStore _formStore;

    public FormsController(IFormStore formStore)
    {
        _formStore = formStore;
    }

    [HttpGet("{id}/styles/active")]
    public async Task<IActionResult> GetActiveStyle(string id, CancellationToken cancellationToken)
    {
        var form = await _formStore.Get(id, cancellationToken);
        if (form == null || form.ActiveStyle == null) return new NoContentResult();
        return Content(form.ActiveStyle.Content, "text/css");
    }

    [HttpGet("last/{correlationId}/styles/active")]
    public async Task<IActionResult> GetLastActiveStyle(string correlationId, CancellationToken cancellationToken)
    {
        var form = await _formStore.GetLatestPublishedVersionByCorrelationId(correlationId, cancellationToken);
        if (form == null || form.ActiveStyle == null) return new NoContentResult();
        return Content(form.ActiveStyle.Content, "text/css");
    }
}