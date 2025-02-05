// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;
using FormBuilder.Models;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Helpers;
using System.Text;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Website.Stores.WorkflowsStore;

public class WorkflowEffects
{
    private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;
    private readonly IdServerWebsiteOptions _options;

    public WorkflowEffects(
        IWebsiteHttpClientFactory websiteHttpClientFactory,
        IOptions<IdServerWebsiteOptions> options)
    {
        _websiteHttpClientFactory = websiteHttpClientFactory;
        _options = options.Value;
    }

    [EffectMethod]
    public async Task Handle(GetWorkflowAction action, IDispatcher dispatcher)
    {
        var workflowsUrl = await GetBaseUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var httpResult = await httpClient.SendAsync(new HttpRequestMessage
        {
            RequestUri = new Uri($"{workflowsUrl}/{action.Id}")
        });
        var json = await httpResult.Content.ReadAsStringAsync();
        var workflowRecord = SidJsonSerializer.Deserialize<WorkflowRecord>(json);
        dispatcher.Dispatch(new GetWorkflowSuccessAction { Workflow = workflowRecord });
    }

    [EffectMethod]
    public async Task Handle(UpdateWorkflowAction action, IDispatcher dispatcher)
    {
        var workflowsUrl = await GetBaseUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var httpResult = await httpClient.SendAsync(new HttpRequestMessage
        {
            RequestUri = new Uri($"{workflowsUrl}/{action.Id}"),
            Content = new StringContent(JsonSerializer.Serialize(action.Workflow), Encoding.UTF8, "application/json"),
            Method = HttpMethod.Put
        });
        await httpResult.Content.ReadAsStringAsync();
        dispatcher.Dispatch(new UpdateWorkflowSuccessAction());
    }


    [EffectMethod]
    public async Task Handle(PublishWorkflowAction action, IDispatcher dispatcher)
    {
        var workflowsUrl = await GetBaseUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var httpResult = await httpClient.SendAsync(new HttpRequestMessage
        {
            RequestUri = new Uri($"{workflowsUrl}/{action.Id}/publish"),
            Method = HttpMethod.Post
        });
        var json = await httpResult.Content.ReadAsStringAsync();
        var workflow = JsonSerializer.Deserialize<WorkflowRecord>(json, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        dispatcher.Dispatch(new PublishWorkflowSuccessAction { Workflow = workflow });
    }


    private async Task<string> GetBaseUrl()
    {
        if (_options.IsReamEnabled)
        {
            var realm = RealmContext.Instance()?.Realm;
            var realmStr = !string.IsNullOrWhiteSpace(realm) ? realm : SimpleIdServer.IdServer.Constants.DefaultRealm;
            return $"{_options.IdServerBaseUrl}/{realmStr}/workflows";
        }

        return $"{_options.IdServerBaseUrl}/workflows";
    }
}

public class GetWorkflowAction
{
    public string Id { get; set; }
}

public class GetWorkflowSuccessAction
{
    public WorkflowRecord Workflow { get; set; }
}

public class UpdateWorkflowAction
{
    public string Id { get; set; }
    public WorkflowRecord Workflow { get; set; }
}

public class UpdateWorkflowSuccessAction
{

}

public class PublishWorkflowAction
{
    public string Id { get; set; }
}

public class PublishWorkflowSuccessAction
{
    public WorkflowRecord Workflow { get; set; }
}