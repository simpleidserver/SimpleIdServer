// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Builders;

public static class SwaggerClientBuilder
{
    public static Client Build(string clientId, string password, List<Scope> additionalScopes, params string[] redirectUrls)
    {
        var scopes = new List<Scope>
        {
            Config.DefaultScopes.Provisioning,
            Config.DefaultScopes.Users,
            Config.DefaultScopes.Acrs,
            Config.DefaultScopes.ConfigurationsScope,
            Config.DefaultScopes.AuthenticationSchemeProviders,
            Config.DefaultScopes.AuthenticationMethods,
            Config.DefaultScopes.RegistrationWorkflows,
            Config.DefaultScopes.ApiResources,
            Config.DefaultScopes.Auditing,
            Config.DefaultScopes.Scopes,
            Config.DefaultScopes.CertificateAuthorities,
            Config.DefaultScopes.Clients,
            Config.DefaultScopes.Realms,
            Config.DefaultScopes.Groups
        };
        scopes.AddRange(additionalScopes);
        return ClientBuilder.BuildTraditionalWebsiteClient(clientId, password, null, redirectUrls).AddScope(scopes.ToArray()).Build();
    }
}
