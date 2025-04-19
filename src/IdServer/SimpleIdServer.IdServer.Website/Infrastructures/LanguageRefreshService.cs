// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Hosting;

namespace SimpleIdServer.IdServer.Website.Infrastructures;

public class LanguageRefreshService : BackgroundService
{
    private readonly ILanguageService _languageService;
    private readonly ILanguageStore _store;
    private readonly TimeSpan _refreshInterval = TimeSpan.FromSeconds(10);

    public LanguageRefreshService(
        ILanguageService languageService, 
        ILanguageStore store)
    {
        _languageService = languageService;
        _store = store;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await RefreshAsync(stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(_refreshInterval, stoppingToken);
            await RefreshAsync(stoppingToken);
        }
    }

    private async Task RefreshAsync(CancellationToken token)
    {
        try
        {
            var codes = await _languageService.GetSupportedLanguagesAsync();
            if (codes != null && codes.Any())
                _store.Update(codes);
        }
        catch
        {
            // log warning ou ignore
        }
    }
}
