// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.IdServer.CredentialIssuer.Parsers
{
    public class SignedJWTCredentialRequest : ICredentialRequest
    {
        public IEnumerable<string> CredentialTypes { get; set; } = new List<string>();
        public IEnumerable<string> ClaimNames { get; set; } = new List<string>();

        public CredentialRequestValidationResult Validate(IEnumerable<AuthorizationData> authorizationData)
        {
            var allTypes = authorizationData.SelectMany(d => d.GetTypes()).Distinct();
            var unsupportedTypes = CredentialTypes.Where(t => !allTypes.Contains(t));
            if (unsupportedTypes.Any()) return CredentialRequestValidationResult.Error(string.Format(ErrorMessages.UNAUTHORIZED_TO_ACCESS, string.Join(",", unsupportedTypes)));
            return CredentialRequestValidationResult.Ok();
        }
    }
}
