// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using AspNetCore.Authentication.ApiKey;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.Scim.ApiKeyAuth;

namespace SimpleIdServer.Scim;

public static class ScimBuilderExtensions
{
    public static ScimBuilder EnableApiKeyAuth(this ScimBuilder builder, ApiKeysConfiguration? apiKeysConfiguration= null)
    {
        if(apiKeysConfiguration != null)
        {
            builder.Services.AddSingleton(ApiKeysConfiguration.Default);
        }
        else
        {
            builder.Services.AddSingleton(apiKeysConfiguration);
        }
        
        builder.Services.AddAuthentication(ApiKeyDefaults.AuthenticationScheme)
                .AddApiKeyInHeaderOrQueryParams<ApiKeyProvider>(options =>
                {
                    options.Realm = "Sample Web API";
                    options.KeyName = "Authorization";
                });
        builder.Services.AddAuthorization(opts => opts.AddDefaultSCIMAuthorizationPolicy());
        return builder;
    }
}
