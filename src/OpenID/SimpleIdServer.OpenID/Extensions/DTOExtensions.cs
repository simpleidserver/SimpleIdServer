// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OpenID.DTOs;

namespace SimpleIdServer.OpenID.Extensions
{
    public static class DTOExtensions
    {
        public static JObject ToDto(this OpenIdClient openidClient, string issuer)
        {
            var result = (openidClient as OAuthClient).ToDto(issuer);
            result.AddNotEmpty(OpenIdClientParameters.ApplicationType, openidClient.ApplicationType)
                .AddNotEmpty(OpenIdClientParameters.SectorIdentifierUri, openidClient.SectorIdentifierUri)
                .AddNotEmpty(OpenIdClientParameters.SubjectType, openidClient.SubjectType)
                .AddNotEmpty(OpenIdClientParameters.IdTokenSignedResponseAlg, openidClient.IdTokenSignedResponseAlg)
                .AddNotEmpty(OpenIdClientParameters.IdTokenEncryptedResponseAlg, openidClient.IdTokenEncryptedResponseAlg)
                .AddNotEmpty(OpenIdClientParameters.IdTokenEncryptedResponseEnc, openidClient.IdTokenEncryptedResponseEnc)
                .AddNotEmpty(OpenIdClientParameters.UserInfoSignedResponseAlg, openidClient.UserInfoSignedResponseAlg)
                .AddNotEmpty(OpenIdClientParameters.UserInfoEncryptedResponseAlg, openidClient.UserInfoEncryptedResponseAlg)
                .AddNotEmpty(OpenIdClientParameters.UserInfoEncryptedResponseEnc, openidClient.UserInfoEncryptedResponseEnc)
                .AddNotEmpty(OpenIdClientParameters.RequestObjectSigningAlg, openidClient.RequestObjectSigningAlg)
                .AddNotEmpty(OpenIdClientParameters.RequestObjectEncryptionAlg, openidClient.RequestObjectEncryptionAlg)
                .AddNotEmpty(OpenIdClientParameters.RequestObjectEncryptionEnc, openidClient.RequestObjectEncryptionEnc)
                .AddNotEmpty(OpenIdClientParameters.DefaultAcrValues, openidClient.DefaultAcrValues)
                .AddNotEmpty(OpenIdClientParameters.PostLogoutRedirectUris, openidClient.PostLogoutRedirectUris);
            if (openidClient.DefaultMaxAge != null)
            {
                result.Add(OpenIdClientParameters.DefaultMaxAge, openidClient.DefaultMaxAge);
            }

            result.Add(OpenIdClientParameters.RequireAuthTime, openidClient.RequireAuthTime);
            return result;
        }
    }
}
