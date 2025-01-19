using FormBuilder.Link;
using FormBuilder.Models;
using FormBuilder.Models.Transformer;
using FormBuilder.Transformers;
using System.Security.Cryptography.Xml;
using System.Text.Json;

namespace FormBuilder.Builders;

public static class WorkflowBuilderExtensions
{
    public static WorkflowBuilder AddLinkPopupAction(this WorkflowBuilder builder, FormRecord sourceForm, FormRecord targetForm, string eltId)
    {
        builder.AddLink(sourceForm, targetForm, eltId, (a) =>
        {
            a.ActionType = WorkflowLinkPopupAction.ActionType;
        });
        return builder;
    }

    public static WorkflowBuilder AddTransformedLinkUrlAction(this WorkflowBuilder builder, FormRecord sourceForm, FormRecord targetForm, string eltId, string url, List<ITransformerParameters> transformers)
    {
        builder.AddLink(sourceForm, targetForm, eltId, (a) =>
        {
            a.ActionType = WorkflowLinkUrlTransformerAction.ActionType;
            a.ActionParameter = JsonSerializer.Serialize(new WorkflowLinkUrlTransformationParameter { Url = url, Transformers = transformers });
        });
        return builder;
    }

    public static WorkflowBuilder AddTransformedLinkUrlActionWithQueryParameter(this WorkflowBuilder builder, FormRecord sourceForm, FormRecord targetForm, string eltId, string url, List<ITransformerParameters> transformers, string queryParameterName, string jsonSource)
    {
        builder.AddLink(sourceForm, targetForm, eltId, (a) =>
        {
            a.ActionType = WorkflowLinkUrlTransformerAction.ActionType;
            a.ActionParameter = JsonSerializer.Serialize(new WorkflowLinkUrlTransformationParameter { Url = url, Transformers = transformers, QueryParameterName = queryParameterName, JsonSource = jsonSource });
        });
        return builder;
    }

    public static WorkflowBuilder AddStaticLinkUrlAction(this WorkflowBuilder builder, FormRecord sourceForm, FormRecord targetForm, string eltId, string url, RegexTransformerParameters transformer = null)
    {
        builder.AddLink(sourceForm, targetForm, eltId, (a) =>
        {
            a.ActionType = WorkflowLinkUrlAction.ActionType;
            a.ActionParameter = JsonSerializer.Serialize(new WorkflowLinkUrlParameter { Url = url });
        });
        return builder;
    }

    public static WorkflowBuilder AddLinkHttpRequestAction(this WorkflowBuilder builder, FormRecord sourceForm, FormRecord targetForm, string eltId, WorkflowLinkHttpRequestParameter parameter)
    {
        builder.AddLink(sourceForm, targetForm, eltId, (a) =>
        {
            a.ActionType = WorkflowLinkHttpRequestAction.ActionType;
            a.ActionParameter = JsonSerializer.Serialize(parameter);
        });
        return builder;
    }

    public static WorkflowBuilder AddLinkAction(this WorkflowBuilder builder, FormRecord sourceForm, FormRecord targetForm, string eltId)
    {
        builder.AddLink(sourceForm, targetForm, eltId, (a) =>
        {
            a.ActionType = WorkflowLinkAction.ActionType;
        });
        return builder;
    }
}
