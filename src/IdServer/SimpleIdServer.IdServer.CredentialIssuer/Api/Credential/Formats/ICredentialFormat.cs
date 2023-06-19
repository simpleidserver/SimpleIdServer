// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Models;
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.CredentialIssuer.Api.Credential.Formats
{
    public interface ICredentialFormat
    {
        string Format { get; }
        JsonObject Transform(CredentialFormatParameter parameter);
    }

    public record CredentialFormatParameter
    {
        public CredentialFormatParameter(Dictionary<string, object> claims, User user, IdentityDocument identityDocument, CredentialTemplate credentialTemplate, string issuer, string cNonce, double cNonceExpiresIn)
        {
            Claims = claims;
            User = user;
            IdentityDocument = identityDocument;
            CredentialTemplate = credentialTemplate;
            Issuer = issuer;
            CNonce= cNonce;
            CNonceExpiresIn = cNonceExpiresIn;
        }

        public Dictionary<string, object> Claims { get; private set; }
        public User User { get; private set; }
        public IdentityDocument IdentityDocument { get; private set; }
        public CredentialTemplate CredentialTemplate { get; private set; }
        public string Issuer { get; private set; }
        public string CNonce { get; private set; }
        public double CNonceExpiresIn { get; private set; }
    }
}
