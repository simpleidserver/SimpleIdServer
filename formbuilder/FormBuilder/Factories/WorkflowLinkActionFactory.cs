using FormBuilder.Link;
using FormBuilder.Models;

namespace FormBuilder.Factories;

public interface IWorkflowLinkActionFactory
{
    List<IWorkflowLinkAction> GetAll(WorkflowRecord workflow, List<FormRecord> forms, WorkflowLink workflowLink);
    IWorkflowLinkAction Build(string actionType);
}

public class WorkflowLinkActionFactory : IWorkflowLinkActionFactory
{
    private readonly List<IWorkflowLinkAction> _actions;

    public WorkflowLinkActionFactory(IEnumerable<IWorkflowLinkAction> actions)
    {
        _actions = actions.ToList();
    }

    public List<IWorkflowLinkAction> GetAll(WorkflowRecord workflow, List<FormRecord> forms, WorkflowLink workflowLink)
    {
        var record = workflow.GetListDataRecord(workflowLink, forms);
        if(record.formElt != null)
            return _actions.Where(a => a.CanBeAppliedMultipleTimes).ToList();

        return _actions.Where(a => !a.ExcludedStepNames.Contains(record.form.Name) && !a.CanBeAppliedMultipleTimes).ToList();
    }

    public IWorkflowLinkAction Build(string actionType)
        => _actions.Single(a => a.Type == actionType);
}
