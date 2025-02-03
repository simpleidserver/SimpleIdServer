using FormBuilder.Models.Layout;

namespace FormBuilder;

public interface IWorkflowLayoutService
{
    string Category { get; }
    WorkflowLayout Get();
}
