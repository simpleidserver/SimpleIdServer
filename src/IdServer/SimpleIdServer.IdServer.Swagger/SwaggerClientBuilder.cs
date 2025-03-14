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
            Constants.DefaultScopes.Provisioning,
            Constants.DefaultScopes.Users,
            Constants.DefaultScopes.Acrs,
            Constants.DefaultScopes.ConfigurationsScope,
            Constants.DefaultScopes.AuthenticationSchemeProviders,
            Constants.DefaultScopes.AuthenticationMethods,
            Constants.DefaultScopes.RegistrationWorkflows,
            Constants.DefaultScopes.ApiResources,
            Constants.DefaultScopes.Auditing,
            Constants.DefaultScopes.Scopes,
            Constants.DefaultScopes.CertificateAuthorities,
            Constants.DefaultScopes.Clients,
            Constants.DefaultScopes.Realms,
            Constants.DefaultScopes.Groups
        };
        scopes.AddRange(additionalScopes);
        return ClientBuilder.BuildTraditionalWebsiteClient(clientId, password, null, redirectUrls).AddScope(scopes.ToArray()).Build();
    }
}
