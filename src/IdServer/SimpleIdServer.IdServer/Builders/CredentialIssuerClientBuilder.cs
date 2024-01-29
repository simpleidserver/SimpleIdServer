// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Builders;

public class CredentialIssuerClientBuilder
{
    private readonly Client _client;

    public CredentialIssuerClientBuilder(Client client)
    {
        _client = client;
    }

    #region Scopes

    public CredentialIssuerClientBuilder AddScope(params Scope[] scopes)
    {
        foreach (var scope in scopes) _client.Scopes.Add(scope);
        return this;
    }

    #endregion

    public CredentialIssuerClientBuilder IsTransactionCodeRequired()
    {
        _client.IsTransactionCodeRequired = true;
        return this;
    }

    public Client Build() => _client;
}
