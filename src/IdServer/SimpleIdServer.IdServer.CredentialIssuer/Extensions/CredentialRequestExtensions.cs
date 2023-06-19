// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.CredentialIssuer.DTOs;
using SimpleIdServer.Vc.Models;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.IdServer.CredentialIssuer.Api.Credential
{
    public static class CredentialRequestExtensions
    {
        public static IEnumerable<string>? GetTypes(this CredentialRequest credentialRequest)
        {
            if (!credentialRequest.OtherParameters.ContainsKey(CredentialRequestNames.Types)) return null;
            var types = credentialRequest.OtherParameters[CredentialRequestNames.Types];
            return types.AsArray().Select(t => t.GetValue<string>());
        }

        public static Dictionary<string, W3CCredentialSubject> GetCredentialSubject(this CredentialRequest credentialRequest)
        {
            return null;
        }
    }
}
