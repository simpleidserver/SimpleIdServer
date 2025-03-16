// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Resources;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.IdServer.Api;

public class OpenIdCredentialValidator
{
    public static void ValidateOpenIdCredential(ICollection<AuthorizationData> authDetails)
    {
        // Logic to validate the OPENID credential : https://openid.github.io/OpenID4VCI/openid-4-verifiable-credential-issuance-wg-draft.html#section-5.1.1
        var openidCredentials = authDetails.Where(t => t.Type == AuthorizationDetailsNames.OpenIdCredential);
        if (!openidCredentials.Any()) return;
        foreach (var openidCredential in openidCredentials)
        {
            if (!openidCredential.AdditionalData.ContainsKey(AuthorizationDetailsNames.CredentialConfigurationId) && 
                !openidCredential.AdditionalData.ContainsKey(AuthorizationDetailsNames.Format))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, AuthorizationDetailsNames.CredentialConfigurationId));
        }
    }
}
