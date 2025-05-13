// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Website.Stores.MigrationStore;

public class MigrationExecutionsEffects
{
    private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;
    private readonly IdServerWebsiteOptions _options;
    private readonly IRealmStore _realmStore;

    public MigrationExecutionsEffects(
        IWebsiteHttpClientFactory websiteHttpClientFactory,
        IOptions<IdServerWebsiteOptions> options,
        IRealmStore realmStore)
    {
        _websiteHttpClientFactory = websiteHttpClientFactory;
        _options = options.Value;
        _realmStore = realmStore;
    }

    [EffectMethod]
    public async Task Handle(GetAllMigrationsExecutionsAction act, IDispatcher dispatcher)
    {
        var baseUrl = await GetMigrationsUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri($"{baseUrl}/executions"),
            Method = HttpMethod.Get
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        var executions = JsonSerializer.Deserialize<List<MigrationExecution>>(json);
        dispatcher.Dispatch(new GetAllMigrationsExecutionsSuccessAction
        {
            Executions = executions
        });
    }

    [EffectMethod]
    public async Task Handle(LaunchMigrationAction act, IDispatcher dispatcher)
    {
        var baseUrl = await GetMigrationsUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri($"{baseUrl}/{act.MigrationName}/launch"),
            Method = HttpMethod.Get
        };
        await httpClient.SendAsync(requestMessage);
        dispatcher.Dispatch(new LaunchMigrationSuccessAction { });
    }

    private Task<string> GetMigrationsUrl() => GetBaseUrl("migrations");

    private async Task<string> GetBaseUrl(string subUrl)
    {
        if (_options.IsReamEnabled)
        {
            var realm = _realmStore.Realm;
            var realmStr = !string.IsNullOrWhiteSpace(realm) ? realm : Constants.DefaultRealm;
            return $"{_options.Issuer}/{realmStr}/{subUrl}";
        }

        return $"{_options.Issuer}/{subUrl}";
    }
}

public class GetAllMigrationsExecutionsAction
{
}

public class GetAllMigrationsExecutionsSuccessAction
{
    public List<MigrationExecution> Executions { get; set; }
}

public class LaunchMigrationAction
{
    public string MigrationName { get; set; }
}

public class LaunchMigrationSuccessAction
{

}