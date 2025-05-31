// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using AspNetCore.Authentication.ApiKey;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.ApiKeyAuth;

public class ApiKeyProvider : IApiKeyProvider
{
    private readonly ILogger<IApiKeyProvider> _logger;
    private readonly ApiKeysConfiguration _configuration;

    public ApiKeyProvider(ILogger<IApiKeyProvider> logger, ApiKeysConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public Task<IApiKey> ProvideAsync(string key)
    {
        const string bearerName = "Bearer";
        IApiKey result = null;
        try
        {
            if (!key.Contains(bearerName, StringComparison.InvariantCultureIgnoreCase)) return Task.FromResult(result);
            var splitted = key.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            if (splitted.Count() != 2) return Task.FromResult(result);
            var expectedKey = splitted[1];
            var storedKey = _configuration.ApiKeys.SingleOrDefault(k => k.Value == expectedKey);
            if (storedKey == null) return Task.FromResult(result);
            result = new ApiKey(key, storedKey.Owner, storedKey.Scopes);
            return Task.FromResult(result);
        }
        catch (System.Exception exception)
        {
            _logger.LogError(exception, exception.Message);
            throw;
        }
    }
}