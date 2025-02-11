using FormBuilder.Helpers;
using FormBuilder.Services;
using FormBuilder.Stores;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace FormBuilder.Controllers;

[Route("forms")]
public class FormsController : Controller
{
    private readonly IFormStore _formStore;
    private readonly IDateTimeHelper _dateTimeHelper;
    private readonly IVersionedFormService _versionedFormService;

    public FormsController(IFormStore formStore, IDateTimeHelper dateTimeHelper, IVersionedFormService versionedFormService)
    {
        _formStore = formStore;
        _dateTimeHelper = dateTimeHelper;
        _versionedFormService = versionedFormService;
    }

    [HttpGet("{id}/styles/active")]
    public async Task<IActionResult> ActiveStyle(string id, CancellationToken cancellationToken)
    {
        var form = await _formStore.Get(id, cancellationToken);
        if (form == null || form.ActiveStyle == null) return new NoContentResult();
        return Content(form.ActiveStyle.Content, "text/css");
    }

    [HttpPut("{id}/styles/active")]
    public async Task<IActionResult> UpdateStyle(string id, [FromBody] UpdateFormStyleCommand cmd, CancellationToken cancellationToken)
    {
        var form = await _formStore.Get(id, cancellationToken);
        if (form == null || form.ActiveStyle == null) return new NoContentResult();
        form.ActiveStyle.Content = cmd.Content;
        await _formStore.SaveChanges(cancellationToken);
        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateFormCommand command, CancellationToken cancellationToken)
    {
        var form = await _formStore.Get(id, cancellationToken);
        if(form == null) return new NoContentResult();
        form.Update(command.Form.Elements.ToList(), _dateTimeHelper.GetCurrent());
        await _formStore.SaveChanges(cancellationToken);
        return new NoContentResult();
    }

    [HttpPost("{id}/publish")]
    public async Task<IActionResult> Publish(string id, CancellationToken cancellationToken)
    {
        var form = await _formStore.Get(id, cancellationToken);
        if (form == null) return new NoContentResult();
        var newForm = await _versionedFormService.Publish(form, cancellationToken);
        return new ContentResult
        {
            StatusCode = (int)HttpStatusCode.Created,
            Content = JsonSerializer.Serialize(newForm),
            ContentType = "application/json"
        };
    }
}