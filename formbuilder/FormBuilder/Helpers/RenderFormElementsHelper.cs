using FormBuilder.Components;
using FormBuilder.Components.FormElements.Input;
using FormBuilder.Components.FormElements.StackLayout;
using FormBuilder.Models;
using Microsoft.AspNetCore.Components.Rendering;

namespace FormBuilder.Helpers;

public interface IRenderFormElementsHelper
{
    void Render(RenderTreeBuilder builder, List<IFormElementRecord> elements, FormViewerContext context, bool isEditModeEnabled);
    void RenderWithZone(RenderTreeBuilder builder, List<IFormElementRecord> elements, FormViewerContext context, bool isEditModeEnabled);
}

public class RenderWithZoneParameter
{
}

public class RenderFormElementsHelper : IRenderFormElementsHelper
{
    private readonly IEnumerable<IFormElementDefinition> _definitions;

    public RenderFormElementsHelper(IEnumerable<IFormElementDefinition> definitions)
    {
        _definitions = definitions;
    }

    public void Render(RenderTreeBuilder builder, List<IFormElementRecord> elements, FormViewerContext context, bool isEditModeEnabled)
    {
        foreach (var record in elements)
            AddElt(builder, record, context, isEditModeEnabled);
    }

    public void RenderWithZone(RenderTreeBuilder builder, List<IFormElementRecord> elements, FormViewerContext context, bool isEditModeEnabled)
    {
        int index = 0;
        foreach (var record in elements)
        {
            var formInputRecord = record as FormInputFieldRecord;
            var stackLayout = record as FormStackLayoutRecord;
            if (isEditModeEnabled && 
                (formInputRecord == null || formInputRecord.Type != FormInputTypes.HIDDEN) &&
                (stackLayout == null || !stackLayout.Elements.Any())
            ) AddZone(builder, elements, index, context);
            AddElt(builder, record, context, isEditModeEnabled);
            index++;
        }
    }

    private void AddElt(RenderTreeBuilder builder, IFormElementRecord record, FormViewerContext context, bool isEditModeEnabled)
    {
        var expectedType = record.GetType();
        var definition = _definitions.Single(d => d.RecordType == expectedType);
        var uiElt = definition.UiElt;
        builder.OpenComponent(0, uiElt);
        builder.AddAttribute(1, "Value", record);
        builder.AddAttribute(2, "Context", context);
        builder.AddAttribute(3, "IsEditModeEnabled", isEditModeEnabled);
        builder.CloseComponent();
    }

    private void AddZone(RenderTreeBuilder builder, List<IFormElementRecord> elements, int index, FormViewerContext context)
    {
        builder.OpenComponent(0, typeof(DropZoneComponent));
        builder.AddAttribute(1, "Parameter", new DropZoneParameter(elements, index));
        builder.AddAttribute(2, "Context", context);
        builder.CloseComponent();
    }
}
