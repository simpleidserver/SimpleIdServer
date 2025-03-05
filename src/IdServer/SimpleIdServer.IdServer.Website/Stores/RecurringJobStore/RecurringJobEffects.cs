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

    private async Task<string> GetBaseUrl()
    {
        if (_options.IsReamEnabled)
        {
            var realm = _realmStore.Realm;
            var realmStr = !string.IsNullOrWhiteSpace(realm) ? realm : SimpleIdServer.IdServer.Constants.DefaultRealm;
            return $"{_options.IdServerBaseUrl}/{realmStr}/recurringjobs";
        }

        return $"{_options.IdServerBaseUrl}/recurringjobs";
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