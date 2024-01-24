// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.CredentialIssuer.Parsers
{
    public interface ICredentialRequest
    {
        IEnumerable<string> CredentialTypes { get; set; }
        IEnumerable<string> ClaimNames { get; set; }
        CredentialRequestValidationResult Validate(IEnumerable<AuthorizationData> authorizationData);

    }

    public class CredentialRequestValidationResult
    {
        private CredentialRequestValidationResult(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; private set; }

        public static CredentialRequestValidationResult Ok() => new CredentialRequestValidationResult(null);

        public static CredentialRequestValidationResult Error(string errorMessage) => new CredentialRequestValidationResult(errorMessage);
    }
}
