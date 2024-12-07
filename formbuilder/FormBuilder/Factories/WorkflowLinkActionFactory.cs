using FormBuilder.Link;

namespace FormBuilder.Factories;

public interface IWorkflowLinkActionFactory
{
    List<IWorkflowLinkAction> GetAll();
    IWorkflowLinkAction Build(string actionType);
}

public class WorkflowLinkActionFactory : IWorkflowLinkActionFactory
{
    private readonly List<IWorkflowLinkAction> _actions;

    public WorkflowLinkActionFactory(IEnumerable<IWorkflowLinkAction> actions)
    {
        _actions = actions.ToList();
    }

    public List<IWorkflowLinkAction> GetAll()
        => _actions;

    public IWorkflowLinkAction Build(string actionType)
        => _actions.Single(a => a.Type == actionType);
}
