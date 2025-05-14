// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Authenticate.Validations;
using SimpleIdServer.IdServer.Domains;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Authenticate.Handlers;

public class OAuthClientSecretBasicAuthenticationHandler : IOAuthClientAuthenticationHandler
{
    private readonly IClientSecretValidator _clientSecretValidator;

    public OAuthClientSecretBasicAuthenticationHandler(IClientSecretValidator clientSecretValidator) 
    {
        _clientSecretValidator = clientSecretValidator;
    }

    public string AuthMethod => AUTH_METHOD;
    public const string AUTH_METHOD = "client_secret_basic";

    public Task<bool> Handle(AuthenticateInstruction authenticateInstruction, Client client, string expectedIssuer, CancellationToken cancellationToken, string errorCode = ErrorCodes.INVALID_CLIENT)
    {
        if (authenticateInstruction == null) throw new ArgumentNullException(nameof(authenticateInstruction));
        if (client == null) throw new ArgumentNullException(nameof(client));
        if (string.IsNullOrWhiteSpace(client.ClientSecret))
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(_clientSecretValidator.IsValid(client, authenticateInstruction.ClientSecretFromAuthorizationHeader));
    }
}
