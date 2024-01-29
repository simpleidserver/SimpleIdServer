// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.IdServer.Api;

public class OpenIdCredentialValidator
{
    public static void ValidateOpenIdCredential(ICollection<AuthorizationData> authDetails)
    {
        // Logic to validate the OPENID credential : https://openid.github.io/OpenID4VCI/openid-4-verifiable-credential-issuance-wg-draft.html#section-5.1.1
        var openidCredentials = authDetails.Where(t => t.Type == Constants.StandardAuthorizationDetails.OpenIdCredential);
        if (!openidCredentials.Any()) return;
        foreach (var openidCredential in openidCredentials)
            if (!openidCredential.AdditionalData.ContainsKey(Constants.StandardAuthorizationDetails.CredentialConfigurationId)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, Constants.StandardAuthorizationDetails.CredentialConfigurationId));
    }
}
