// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.DTOs;
using System.Linq;

namespace SimpleIdServer.OpenID.Extensions
{
    public static class DTOExtensions
    {
        public static JObject ToDto(this OpenIdClient openidClient, string issuer)
        {
            var result = (openidClient as BaseClient).ToDto(issuer);
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
                .AddNotEmpty(OpenIdClientParameters.InitiateLoginUri, openidClient.InitiateLoginUri)
                .AddNotEmpty(OpenIdClientParameters.PostLogoutRedirectUris, openidClient.PostLogoutRedirectUris);
            if (openidClient.DefaultMaxAge != null)
            {
                result.Add(OpenIdClientParameters.DefaultMaxAge, openidClient.DefaultMaxAge);
            }

            result.Add(OpenIdClientParameters.RequireAuthTime, openidClient.RequireAuthTime);
            result.Add(OpenIdClientParameters.ApplicationKind, (int)openidClient.ApplicationKind);
            return result;
        }

        public static OpenIdClient ToDomain(this JObject jObj)
        {
            var result = new OpenIdClient();
            jObj.EnrichDomain(result);
            result.ApplicationType = jObj.GetApplicationType();
            result.SectorIdentifierUri = jObj.GetSectorIdentifierUri();
            result.SubjectType = jObj.GetSubjectType();
            result.IdTokenSignedResponseAlg = jObj.GetIdTokenSignedResponseAlg();
            result.IdTokenEncryptedResponseAlg = jObj.GetIdTokenEncryptedResponseAlg();
            result.IdTokenEncryptedResponseEnc = jObj.GetIdTokenEncryptedResponseEnc();
            result.UserInfoSignedResponseAlg = jObj.GetUserInfoSignedResponseAlg();
            result.UserInfoEncryptedResponseAlg = jObj.GetUserInfoEncryptedResponseAlg();
            result.UserInfoEncryptedResponseEnc = jObj.GetUserInfoEncryptedResponseEnc();
            result.RequestObjectSigningAlg = jObj.GetRequestObjectSigningAlg();
            result.RequestObjectEncryptionAlg = jObj.GetRequestObjectEncryptionAlg();
            result.RequestObjectEncryptionEnc = jObj.GetRequestObjectEncryptionEnc();
            result.DefaultMaxAge = jObj.GetDefaultMaxAge();
            result.RequireAuthTime = jObj.GetRequireAuhTime() ?? false;
            result.DefaultAcrValues = jObj.GetDefaultAcrValues();
            result.PostLogoutRedirectUris = jObj.GetPostLogoutRedirectUris();
            result.InitiateLoginUri = jObj.GetInitiateLoginUri();
            result.ApplicationKind = jObj.GetApplicationKind();
            result.AllowedScopes = result.AllowedScopes.Select(s => new OAuthScope
            {
                Name = s.Name
            }).ToList();
            return result;
        }

        public static CreateOpenIdClientParameter ToCreateOpenIdClientParameter(this JObject jObj)
        {
            return new CreateOpenIdClientParameter
            {
                ApplicationKind = jObj.GetApplicationKind(),
                ClientName = jObj.GetClientName()
            };
        }
    }
}
