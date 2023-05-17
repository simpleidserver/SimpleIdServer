// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Vc.DTOs;

namespace SimpleIdServer.IdServer.CredentialIssuer.Api.Credential.Parsers
{
    public class SignedJWTCredentialRequestParser : ICredentialRequestParser
    {
        public string Format => Vc.Constants.CredentialTemplateProfiles.W3CVerifiableCredentials;

        public ExtractionResult Extract(CredentialRequest request)
        {
            if (!request.OtherParameters.ContainsKey(W3CCredentialTemplateNames.Types)) return ExtractionResult.Error(string.Format(ErrorMessages.MISSING_PARAMETER, W3CCredentialTemplateNames.Types));
            return ExtractionResult.Ok();
        }
    }
}
