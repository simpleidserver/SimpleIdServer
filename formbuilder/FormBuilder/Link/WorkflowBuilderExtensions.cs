using FormBuilder.Conditions;
using FormBuilder.Link;
using FormBuilder.Models;
using FormBuilder.Models.Transformer;
using FormBuilder.Transformers;
using System.Text.Json;

namespace FormBuilder.Builders;

public static class WorkflowBuilderExtensions
{
    public static WorkflowBuilder AddLinkPopupAction(this WorkflowBuilder builder, FormRecord sourceForm, FormRecord targetForm, string eltId, string description, bool isMainLink)
    {
        builder.AddLink(sourceForm, targetForm, eltId, description, isMainLink, (a) =>
        {
            a.ActionType = WorkflowLinkPopupAction.ActionType;
        });
        return builder;
    }

    public static WorkflowBuilder AddTransformedLinkUrlAction(this WorkflowBuilder builder, FormRecord sourceForm, FormRecord targetForm, string eltId, string description, string url, List<ITransformerParameters> transformers, bool isMainLink)
    {
        builder.AddLink(sourceForm, targetForm, eltId, description, isMainLink, (a) =>
        {
            a.ActionType = WorkflowLinkUrlTransformerAction.ActionType;
            a.ActionParameter = JsonSerializer.Serialize(new WorkflowLinkUrlTransformationParameter { Url = url, Transformers = transformers });
        });
        return builder;
    }

    public static WorkflowBuilder AddTransformedLinkUrlActionWithQueryParameter(this WorkflowBuilder builder, FormRecord sourceForm, FormRecord targetForm, string eltId, string description, string url, List<ITransformerParameters> transformers, string queryParameterName, string jsonSource, bool isMainLink)
    {
        builder.AddLink(sourceForm, targetForm, eltId, description, isMainLink, (a) =>
        {
            a.ActionType = WorkflowLinkUrlTransformerAction.ActionType;
            a.ActionParameter = JsonSerializer.Serialize(new WorkflowLinkUrlTransformationParameter { Url = url, Transformers = transformers, QueryParameterName = queryParameterName, JsonSource = jsonSource });
        });
        return builder;
    }

    public static WorkflowBuilder AddStaticLinkUrlAction(this WorkflowBuilder builder, FormRecord sourceForm, FormRecord targetForm, string eltId, string description, string url, bool isMainLink, RegexTransformerParameters transformer = null)
    {
        builder.AddLink(sourceForm, targetForm, eltId, description, isMainLink, (a) =>
        {
            a.ActionType = WorkflowLinkUrlAction.ActionType;
            a.ActionParameter = JsonSerializer.Serialize(new WorkflowLinkUrlParameter { Url = url });
        });
        return builder;
    }

    public static WorkflowBuilder AddLinkHttpRequestAction(this WorkflowBuilder builder, FormRecord sourceForm, FormRecord targetForm, string eltId, string description, WorkflowLinkHttpRequestParameter parameter, bool isMainLink)
    {
        builder.AddLink(sourceForm, targetForm, eltId, description, isMainLink, (a) =>
        {
            a.ActionType = WorkflowLinkHttpRequestAction.ActionType;
            a.ActionParameter = JsonSerializer.Serialize(parameter);
        });
        return builder;
    }

    public static WorkflowBuilder AddLinkHttpRequestAction(this WorkflowBuilder builder, FormRecord sourceForm, List<(FormRecord form, IConditionParameter condition, string description)> targets, string eltId, WorkflowLinkHttpRequestParameter parameter, bool isMainLink)
    {
        builder.AddLink(sourceForm, targets, eltId, isMainLink, (a) =>
        {
            a.ActionType = WorkflowLinkHttpRequestAction.ActionType;
            a.ActionParameter = JsonSerializer.Serialize(parameter);
        });
        return builder;
    }

    public static WorkflowBuilder AddLinkAction(this WorkflowBuilder builder, FormRecord sourceForm, FormRecord targetForm, string eltId, string description, bool isMainLink)
    {
        builder.AddLink(sourceForm, targetForm, eltId, description, isMainLink, (a) =>
        {
            a.ActionType = WorkflowLinkAction.ActionType;
        });
        return builder;
    }
}
