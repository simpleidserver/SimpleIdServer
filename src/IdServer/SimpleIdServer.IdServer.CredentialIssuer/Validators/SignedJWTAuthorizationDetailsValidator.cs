// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.CredentialIssuer.DTOs;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Exceptions;

namespace SimpleIdServer.IdServer.CredentialIssuer.Validators
{
    public class SignedJWTAuthorizationDetailsValidator : ICredentialAuthorizationDetailsValidator
    {
        public string Format => Vc.Constants.CredentialTemplateProfiles.W3CVerifiableCredentials;

        public void Validate(AuthorizationData authorizationData)
        {
            var types = authorizationData.GetTypes();
            if (types == null) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, AuthorizationDataParameters.Types));
        }
    }
}
