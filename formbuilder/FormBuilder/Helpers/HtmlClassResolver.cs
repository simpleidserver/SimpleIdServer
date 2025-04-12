using FormBuilder.Components;
using FormBuilder.Models;

namespace FormBuilder.Helpers;

public interface IHtmlClassResolver
{
    string Resolve(IElement record, string eltName, WorkflowContext context);
}

public class HtmlClassResolver : IHtmlClassResolver
{
    public string Resolve(IElement record, string eltName, WorkflowContext context)
    {
        var cl = record.Classes
            .SingleOrDefault(c => c.Element == eltName && c.TemplateName == context.TemplateName)
            ?.Value;
        return cl ?? string.Empty;
    }
}
