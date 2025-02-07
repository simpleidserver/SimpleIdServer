using FormBuilder.Components.Drag;
using FormBuilder.Models;
namespace FormBuilder.Components.FormElements;

public interface IGenericFormElement<T> where T : IFormElementRecord
{
    T Value { get; set; }
    ParentEltContext ParentContext { get; set; }
    WorkflowContext Context { get; set; }
    bool IsEditModeEnabled { get; set; }
}