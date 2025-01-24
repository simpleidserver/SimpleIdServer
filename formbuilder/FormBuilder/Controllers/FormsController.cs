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
    public async Task<IActionResult> ActiveStyle(string id, int version, CancellationToken cancellationToken)
    {
        var form = await _formStore.Get(id, cancellationToken);
        if (form == null || form.ActiveStyle == null) return new NoContentResult();
        return Content(form.ActiveStyle.Content, "text/css");
    }

    [HttpPut("{id}/{version}/styles/active")]
    public async Task<IActionResult> UpdateStyle(string id, [FromBody] UpdateFormStyleCommand cmd, CancellationToken cancellationToken)
    {
        var form = await _formStore.Get(id, cancellationToken);
        if (form == null || form.ActiveStyle == null) return new NoContentResult();
        form.ActiveStyle.Content = cmd.Content;
        await _formStore.SaveChanges(cancellationToken);
        return NoContent();
    }
}