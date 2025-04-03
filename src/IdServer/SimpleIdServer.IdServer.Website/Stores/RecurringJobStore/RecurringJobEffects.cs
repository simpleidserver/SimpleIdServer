// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.RecurringJobs;
using SimpleIdServer.IdServer.Helpers;
using System.Text;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Website.Stores.RecurringJobStore;

public class RecurringJobEffects
{
    private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;
    private readonly IdServerWebsiteOptions _options;
    private readonly IRealmStore _realmStore;

    public RecurringJobEffects(IWebsiteHttpClientFactory websiteHttpClientFactory, IOptions<IdServerWebsiteOptions> options, IRealmStore realmStore)
    {
        _websiteHttpClientFactory = websiteHttpClientFactory;
        _options = options.Value;
        _realmStore = realmStore;
    }

    [EffectMethod]
    public async Task Handle(GetRecurringJobsAction action, IDispatcher dispatcher)
    {
        var url = await GetBaseUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri(url),
            Method = HttpMethod.Get
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var recurringJobs = JsonSerializer.Deserialize<List<RecurringJobResult>>(json, options);
        dispatcher.Dispatch(new GetRecurringJobsSuccessAction { RecurringJobs = recurringJobs });
    }

    [EffectMethod]
    public async Task Handle(UpdateRecurringJobAction action, IDispatcher dispatcher)
    {
        var url = await GetBaseUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri($"{url}/{action.Id}"),
            Method = HttpMethod.Put,
            Content = new StringContent(JsonSerializer.Serialize(new UpdateJobParameter { Cron = action.Cron }), Encoding.UTF8, "application/json")
        };
         await httpClient.SendAsync(requestMessage);
        dispatcher.Dispatch(new UpdateRecurringJobSuccessAction { Cron = action.Cron, Id = action.Id });
    }

    [EffectMethod]
    public async Task Handle(LaunchRecurringJobAction action, IDispatcher dispatcher)
    {
        var url = await GetBaseUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri($"{url}/{action.Id}/launch"),
            Method = HttpMethod.Get
        };
        await httpClient.SendAsync(requestMessage);
        dispatcher.Dispatch(new LaunchRecurringJobSuccessAction { Id = action.Id });
    }

    [EffectMethod]
    public async Task Handle(EnableRecurringJobAction action, IDispatcher dispatcher)
    {
        var url = await GetBaseUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri($"{url}/{action.Id}/enable"),
            Method = HttpMethod.Post
        };
        await httpClient.SendAsync(requestMessage);
        dispatcher.Dispatch(new EnableRecurringJobSuccessAction { Id = action.Id });
    }

    [EffectMethod]
    public async Task Handle(DisableRecurringJobAction action, IDispatcher dispatcher)
    {
        var url = await GetBaseUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri($"{url}/{action.Id}/disable"),
            Method = HttpMethod.Delete
        };
        await httpClient.SendAsync(requestMessage);
        dispatcher.Dispatch(new DisableRecurringJobSuccessAction { Id = action.Id });
    }

    [EffectMethod]
    public async Task Handle(GetLastFailedJobsAction action, IDispatcher dispatcher)
    {
        var url = await GetBaseUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri($"{url}/failedjobs"),
            Method = HttpMethod.Get
        };
        var result = await httpClient.SendAsync(requestMessage);
        var json = await result.Content.ReadAsStringAsync();
        var failedJobs = JsonSerializer.Deserialize<List<FailedJobResult>>(json, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        dispatcher.Dispatch(new GetLastFailedJobsSuccessAction { FailedJobs = failedJobs });
    }

    private async Task<string> GetBaseUrl()
    {
        if (_options.IsReamEnabled)
        {
            var realm = _realmStore.Realm;
            var realmStr = !string.IsNullOrWhiteSpace(realm) ? realm : SimpleIdServer.IdServer.Constants.DefaultRealm;
            return $"{_options.Issuer}/{realmStr}/recurringjobs";
        }

        return $"{_options.Issuer}/recurringjobs";
    }
}

public class GetRecurringJobsAction
{

}

public class GetRecurringJobsSuccessAction
{
    public List<RecurringJobResult> RecurringJobs
    {
        get; set;
    }
}

public class  UpdateRecurringJobAction
{
    public string Id
    {
        get; set;
    }

    public string Cron
    {
        get; set;
    }
}

public class UpdateRecurringJobSuccessAction
{
    public string Id 
    { 
        get; set; 
    }

    public string Cron 
    { 
        get; set; 
    }
}

public class LaunchRecurringJobAction
{
    public string Id
    {
        get; set;
    }
}

public class LaunchRecurringJobSuccessAction
{
    public string Id
    {
        get; set;
    }
}

public class EnableRecurringJobAction
{
    public string Id
    {
        get; set;
    }
}

public class EnableRecurringJobSuccessAction
{
    public string Id
    {
        get; set;
    }
}

public class DisableRecurringJobAction
{
    public string Id
    {
        get; set;
    }
}

public class DisableRecurringJobSuccessAction
{
    public string Id
    {
        get; set;
    }
}

public class GetLastFailedJobsAction
{

}

public class GetLastFailedJobsSuccessAction
{
    public List<FailedJobResult> FailedJobs
    {
        get; set;
    }
}