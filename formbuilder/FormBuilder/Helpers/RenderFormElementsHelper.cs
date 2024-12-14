using FormBuilder.Components;
using FormBuilder.Components.Drag;
using FormBuilder.Models;
using Microsoft.AspNetCore.Components.Rendering;
using System.Collections.ObjectModel;

namespace FormBuilder.Helpers;

public interface IRenderFormElementsHelper
{
    void Render(RenderTreeBuilder builder, ObservableCollection<IFormElementRecord> elements, FormViewerContext context, bool isEditModeEnabled, WorkflowViewerContext workflowContext, bool isInteractableElementEnabled, WorkflowExecutionContext workflowExecutionContext);
}

public class RenderFormElementsHelper : IRenderFormElementsHelper
{
    private readonly IEnumerable<IFormElementDefinition> _definitions;

    public RenderFormElementsHelper(IEnumerable<IFormElementDefinition> definitions)
    {
        _definitions = definitions;
    }

    public void Render(RenderTreeBuilder builder, ObservableCollection<IFormElementRecord> elements, FormViewerContext context, bool isEditModeEnabled, WorkflowViewerContext workflowContext, bool isInteractableElementEnabled, WorkflowExecutionContext workflowExecutionContext)
    {
        var i = 0;
        foreach (var record in elements)
        {
            var parentEltCtx = new ParentEltContext(elements, i);
            AddElt(builder, record, context, parentEltCtx, isEditModeEnabled, workflowContext, isInteractableElementEnabled, workflowExecutionContext);
            i++;
        }
    }

    private void AddElt(RenderTreeBuilder builder, IFormElementRecord record, FormViewerContext context, ParentEltContext parentEltContext, bool isEditModeEnabled, WorkflowViewerContext workflowContext, bool isInteractableElementEnabled, WorkflowExecutionContext workflowExecutionContext)
    {
        var expectedType = record.GetType();
        var definition = _definitions.Single(d => d.RecordType == expectedType);
        var uiElt = definition.UiElt;
        builder.OpenComponent(0, uiElt);
        builder.AddAttribute(1, "Value", record);
        builder.AddAttribute(2, "Context", context);
        builder.AddAttribute(3, "IsEditModeEnabled", isEditModeEnabled);
        builder.AddAttribute(4, "ParentContext", parentEltContext);
        builder.AddAttribute(5, "WorkflowContext", workflowContext);
        builder.AddAttribute(6, "IsInteractableElementEnabled", isInteractableElementEnabled);
        builder.AddAttribute(7, "WorkflowExecutionContext", workflowExecutionContext);
        builder.CloseComponent();
    }
}
