// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Authenticate.Handlers
{
    public class OAuthClientSecretPostAuthenticationHandler : IOAuthClientAuthenticationHandler
    {
        public OAuthClientSecretPostAuthenticationHandler() { }

        public string AuthMethod => "client_secret_post";

        public Task<bool> Handle(AuthenticateInstruction authenticateInstruction, BaseClient client, string expectedIssuer, CancellationToken cancellationToken, string errorCode = ErrorCodes.INVALID_CLIENT)
        {
            if (authenticateInstruction == null)
            {
                throw new ArgumentNullException(nameof(authenticateInstruction));
            }

            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (string.IsNullOrWhiteSpace(client.ClientSecret))
            {
                return Task.FromResult(false);
            }

            var result = string.Compare(client.ClientSecret, authenticateInstruction.ClientSecretFromHttpRequestBody, StringComparison.CurrentCultureIgnoreCase) == 0;
            return Task.FromResult(result);
        }
    }
}
