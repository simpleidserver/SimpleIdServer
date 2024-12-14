using FormBuilder.Components.Drag;
using FormBuilder.Models;
namespace FormBuilder.Components.FormElements;

public interface IGenericFormElement<T> where T : IFormElementRecord
{
    T Value { get; set; }
    ParentEltContext ParentContext { get; set; }
    FormViewerContext Context { get; set; }
    WorkflowViewerContext WorkflowContext { get; set; }
    WorkflowExecutionContext WorkflowExecutionContext { get; set; }
    bool IsEditModeEnabled { get; set; }
    bool IsInteractableElementEnabled { get; set; }
}