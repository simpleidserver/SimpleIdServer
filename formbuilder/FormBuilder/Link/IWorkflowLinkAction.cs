using FormBuilder.Components;
using FormBuilder.Models;

namespace FormBuilder.Link;

public interface IWorkflowLinkAction
{
    string Type { get; }
    Task Execute(WorkflowLink activeLink, WorkflowExecutionContext context);
}
