// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Api.Token.Handlers;
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

    #region Grant types

    /// <summary>
    /// Allows the client to use client_credentials grant-type.
    /// </summary>
    /// <returns></returns>
    public CredentialIssuerClientBuilder EnableClientGrantType()
    {
        _client.GrantTypes.Add(ClientCredentialsHandler.GRANT_TYPE);
        return this;
    }

    #endregion

    #region Other parameters

    /// <summary>
    /// Set client name.
    /// </summary>
    /// <param name="clientName"></param>
    /// <returns></returns>
    public CredentialIssuerClientBuilder SetClientName(string clientName, string language = null)
    {
        if (string.IsNullOrWhiteSpace(language))
            language = Domains.Language.Default;

        _client.Translations.Add(new Translation
        {
            Key = "client_name",
            Value = clientName,
            Language = language
        });
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
