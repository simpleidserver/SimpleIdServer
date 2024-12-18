using FormBuilder.Components;
using FormBuilder.Components.Drag;
using FormBuilder.Models;
using Microsoft.AspNetCore.Components.Rendering;
using System.Collections.ObjectModel;

namespace FormBuilder.Helpers;

public interface IRenderFormElementsHelper
{
    void RenderCurrentStep(RenderTreeBuilder builder, WorkflowContext context, bool isEditModeEnabled, bool isInteractableElementEnabled);
    void Render(RenderTreeBuilder builder, ObservableCollection<IFormElementRecord> elements, WorkflowContext context, bool isEditModeEnabled, bool isInteractableElementEnabled);
}

public class RenderFormElementsHelper : IRenderFormElementsHelper
{
    private readonly IEnumerable<IFormElementDefinition> _definitions;

    public RenderFormElementsHelper(IEnumerable<IFormElementDefinition> definitions)
    {
        _definitions = definitions;
    }

    public void RenderCurrentStep(RenderTreeBuilder builder, WorkflowContext context, bool isEditModeEnabled, bool isInteractableElementEnabled)
    {
        var currentForm = context.GetCurrentFormRecord();
        Render(builder, currentForm.Elements, context, isEditModeEnabled, isInteractableElementEnabled);
    }

    public void Render(RenderTreeBuilder builder, ObservableCollection<IFormElementRecord> elements, WorkflowContext context, bool isEditModeEnabled, bool isInteractableElementEnabled)
    {
        var i = 0;
        foreach (var record in elements)
        {
            var parentEltCtx = new ParentEltContext(elements, i);
            AddElt(builder, record, context, parentEltCtx, isEditModeEnabled, isInteractableElementEnabled);
            i++;
        }
    }

    private void AddElt(RenderTreeBuilder builder, IFormElementRecord record, WorkflowContext context, ParentEltContext parentEltContext, bool isEditModeEnabled, bool isInteractableElementEnabled)
    {
        var expectedType = record.GetType();
        var definition = _definitions.Single(d => d.RecordType == expectedType);
        var uiElt = definition.UiElt;
        builder.OpenComponent(0, uiElt);
        builder.AddAttribute(1, "Value", record);
        builder.AddAttribute(2, "Context", context);
        builder.AddAttribute(3, "IsEditModeEnabled", isEditModeEnabled);
        builder.AddAttribute(4, "ParentContext", parentEltContext);
        builder.AddAttribute(5, "IsInteractableElementEnabled", isInteractableElementEnabled);
        builder.CloseComponent();
    }
}
