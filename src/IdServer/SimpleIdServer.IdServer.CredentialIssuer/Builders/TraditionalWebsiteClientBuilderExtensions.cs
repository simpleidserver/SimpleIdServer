// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Builders
{
    public static class TraditionalWebsiteClientBuilderExtensions
    {

        #region Credential Issuance

        /// <summary>
        /// Add openid_credential into AuthorizationDataTypes.
        /// </summary>
        /// <returns></returns>
        public static TraditionalWebsiteClientBuilder TrustOpenIdCredential(this TraditionalWebsiteClientBuilder builder)
        {
            builder.Client.AuthorizationDataTypes.Add(IdServer.CredentialIssuer.Constants.StandardAuthorizationDetails.OpenIdCredential);
            return builder;
        }

        #endregion
    }
}
