using FormBuilder.Components;
using FormBuilder.Components.Drag;
using FormBuilder.Models;
using Microsoft.AspNetCore.Components.Rendering;
using System.Collections.ObjectModel;

namespace FormBuilder.Helpers;

public interface IRenderFormElementsHelper
{
    void RenderCurrentStep(RenderTreeBuilder builder, WorkflowContext context, bool isEditModeEnabled);
    void Render(RenderTreeBuilder builder, bool isParentBlocked, int parentLevel, ObservableCollection<IFormElementRecord> elements, WorkflowContext context, bool isEditModeEnabled);
}

public class RenderFormElementsHelper : IRenderFormElementsHelper
{
    private readonly IEnumerable<IFormElementDefinition> _definitions;

    public RenderFormElementsHelper(IEnumerable<IFormElementDefinition> definitions)
    {
        _definitions = definitions;
    }

    public void RenderCurrentStep(RenderTreeBuilder builder, WorkflowContext context, bool isEditModeEnabled)
    {
        var currentForm = context.GetCurrentFormRecord();
        Render(builder, false, 0, currentForm.Elements, context, isEditModeEnabled);
    }

    public void Render(RenderTreeBuilder builder, bool isParentBlocked, int parentLevel, ObservableCollection<IFormElementRecord> elements, WorkflowContext context, bool isEditModeEnabled)
    {
        var i = 0;
        foreach (var record in elements)
        {
            var parentEltCtx = new ParentEltContext(isParentBlocked, parentLevel, elements, i);
            AddElt(builder, record, context, parentEltCtx, isEditModeEnabled);
            i++;
        }
    }

    private void AddElt(RenderTreeBuilder builder, IFormElementRecord record, WorkflowContext context, ParentEltContext parentEltContext, bool isEditModeEnabled)
    {
        var expectedType = record.GetType();
        var definition = _definitions.Single(d => d.RecordType == expectedType);
        var uiElt = definition.UiElt;
        builder.OpenComponent(0, uiElt);
        builder.AddAttribute(1, "Value", record);
        builder.AddAttribute(2, "Context", context);
        builder.AddAttribute(3, "IsEditModeEnabled", isEditModeEnabled);
        builder.AddAttribute(4, "ParentContext", parentEltContext);
        builder.CloseComponent();
    }
}
