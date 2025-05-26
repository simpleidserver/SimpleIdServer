using FormBuilder.Factories;
using FormBuilder.Models;
using System.Text.Json.Nodes;

namespace FormBuilder.Helpers;

public interface IWorkflowLinkHelper
{
    string ResolveNextStep(JsonObject input, WorkflowRecord workflow, string currentLink);
}

public class WorkflowLinkHelper : IWorkflowLinkHelper
{
    private readonly IConditionRuleEngineFactory _conditionRuleEngineFactory;

    public WorkflowLinkHelper(IConditionRuleEngineFactory conditionRuleEngineFactory)
    {
        _conditionRuleEngineFactory = conditionRuleEngineFactory;
    }

    public string ResolveNextStep(JsonObject input, WorkflowRecord workflow, string currentLink)
    {
        var link = workflow.Links.Single(l => l.Id == currentLink);
        var targets = link.Targets;
        foreach(var target in targets.Where(t => t.Condition != null))
        {
            if (_conditionRuleEngineFactory.Evaluate(input, target.Condition))
            {
                return target.TargetStepId;
            }
        }

        var defaultLink = targets.Single(t => t.Condition == null);
        return defaultLink.TargetStepId;
    }
}
