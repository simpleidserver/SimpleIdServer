// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains.Clients;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Authenticate.Handlers
{
    public class OAuthClientTlsAuthenticationHandler : IOAuthClientAuthenticationHandler
    {
        public string AuthMethod => "tls_client_auth";

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

            if (authenticateInstruction.Certificate == null || client.Secrets == null)
            {
                return Task.FromResult(false);
            }

            var thumbPrint = client.Secrets.FirstOrDefault(s => s.Type == ClientSecretTypes.X509Thumbprint);
            var name = client.Secrets.FirstOrDefault(s => s.Type == ClientSecretTypes.X509Name);
            if (thumbPrint == null || name == null)
            {
                return Task.FromResult(false);
            }

            var certificate = authenticateInstruction.Certificate;
            var isSameThumbPrint = thumbPrint == null || thumbPrint != null && thumbPrint.Value == certificate.Thumbprint;
            var isSameName = name == null || name != null && name.Value == certificate.SubjectName.Name;
            return Task.FromResult(isSameName && isSameThumbPrint);
        }
    }
}