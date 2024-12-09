using FormBuilder.Link;
using FormBuilder.Models;
using System.Text.Json;

namespace FormBuilder.Builders;

public static class WorkflowBuilderExtensions
{
    public static WorkflowBuilder AddLinkPopupAction(this WorkflowBuilder builder, FormRecord sourceForm, FormRecord targetForm, string eltId)
    {
        builder.AddLink(sourceForm, targetForm, eltId, (a) =>
        {
            a.ActionParameter = WorkflowLinkPopupAction.ActionType;
        });
        return builder;
    }

    public static WorkflowBuilder AddLinkUrlAction(this WorkflowBuilder builder, FormRecord sourceForm, FormRecord targetForm, string eltId, string url)
    {
        builder.AddLink(sourceForm, targetForm, eltId, (a) =>
        {
            a.ActionParameter = WorkflowLinkUrlAction.ActionType;
            a.ActionParameter = JsonSerializer.Serialize(new WorkflowLinkUrlParameter { Url = url });
        });
        return builder;
    }

    public static WorkflowBuilder AddLinkHttpRequestAction(this WorkflowBuilder builder, FormRecord sourceForm, FormRecord targetForm, string eltId, WorkflowLinkHttpRequestParameter parameter)
    {
        builder.AddLink(sourceForm, targetForm, eltId, (a) =>
        {
            a.ActionParameter = WorkflowLinkHttpRequestAction.ActionType;
            a.ActionParameter = JsonSerializer.Serialize(parameter);
        });
        return builder;
    }
}
