// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains.Clients;
using SimpleIdServer.OAuth.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Authenticate.Handlers
{
    public class OAuthClientSecretBasicAuthenticationHandler : IOAuthClientAuthenticationHandler
    {
        public OAuthClientSecretBasicAuthenticationHandler() { }

        public string AuthMethod => AUTH_METHOD;
        public const string AUTH_METHOD = "client_secret_basic";

        public Task<bool> Handle(AuthenticateInstruction authenticateInstruction, OAuthClient client, string expectedIssuer)
        {
            if (authenticateInstruction == null)
            {
                throw new ArgumentNullException(nameof(authenticateInstruction));
            }

            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (client.Secrets == null)
            {
                return Task.FromResult(false);
            }

            var clientSecret = client.Secrets.FirstOrDefault(s => s.Type == ClientSecretTypes.SharedSecret);
            if (clientSecret == null)
            {
                return Task.FromResult(false);
            }

            var result = string.Compare(clientSecret.Value, PasswordHelper.ComputeHash(authenticateInstruction.ClientSecretFromAuthorizationHeader), StringComparison.CurrentCultureIgnoreCase) == 0;
            return Task.FromResult(true);
        }
    }
}
