using FormBuilder.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace FormBuilder.Helpers;

public interface IRenderFormElementsHelper
{
    void Render(RenderTreeBuilder builder, IEnumerable<IFormElementRecord> elements, AntiforgeryTokenRecord antiforgeryToken);
}

public class RenderFormElementsHelper : IRenderFormElementsHelper
{
    private readonly IEnumerable<IFormElementDefinition> _definitions;

    public RenderFormElementsHelper(IEnumerable<IFormElementDefinition> definitions)
    {
        _definitions = definitions;
    }

    public void Render(RenderTreeBuilder builder, IEnumerable<IFormElementRecord> elements, AntiforgeryTokenRecord antiforgeryToken)
    {
        foreach (var record in elements)
        {
            var expectedType = record.GetType();
            var definition = _definitions.Single(d => d.RecordType == expectedType);
            var uiElt = definition.UiElt;
            builder.OpenComponent(0, uiElt);
            builder.AddAttribute(1, "Value", record);
            builder.AddAttribute(2, "AntiforgeryToken", antiforgeryToken);
            builder.CloseComponent();
        }
    }
}
