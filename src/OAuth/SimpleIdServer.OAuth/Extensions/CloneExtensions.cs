// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Domains.Clients;
using SimpleIdServer.OAuth.Domains.Scopes;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.OAuth.Extensions
{
    public static class CloneExtensions
    {
        public static JsonWebKey Copy(this JsonWebKey jsonWebKey)
        {
            return new JsonWebKey
            {
                Alg = jsonWebKey.Alg,
                KeyOps = jsonWebKey.KeyOps == null ? new KeyOperations[0] : jsonWebKey.KeyOps.ToList().ToArray(),
                Kid = jsonWebKey.Kid,
                Kty = jsonWebKey.Kty,
                SerializedKey = jsonWebKey.SerializedKey,
                Use = jsonWebKey.Use,
                Content = jsonWebKey.Content.ToDictionary(s => s.Key, s => s.Value)
            };
        }

        public static OAuthClient Copy(this OAuthClient client)
        {
            return new OAuthClient
            {
                AllowedScopes = client.AllowedScopes == null ? new List<OAuthScope>() : client.AllowedScopes.Select(s => s.Copy()).ToList(),
                ApplicationType = client.ApplicationType,
                ClientId = client.ClientId,
                ClientNames = client.ClientNames == null ? new List<OAuthTranslation>() : client.ClientNames.Select(s => (OAuthTranslation)s.Clone()).ToList(),
                ClientUris = client.ClientUris == null ? new List<OAuthTranslation>() : client.ClientUris.Select(s => (OAuthTranslation)s.Clone()).ToList(),
                LogoUris = client.LogoUris == null ? new List<OAuthTranslation>() : client.LogoUris.Select(s => (OAuthTranslation)s.Clone()).ToList(),
                PolicyUris = client.PolicyUris == null ? new List<OAuthTranslation>() : client.PolicyUris.Select(s => (OAuthTranslation)s.Clone()).ToList(),
                TosUris = client.TosUris == null ? new List<OAuthTranslation>() : client.TosUris.Select(s => (OAuthTranslation)s.Clone()).ToList(),
                Contacts = client.Contacts == null ? new List<string>() : client.Contacts.ToList(),
                CreateDateTime = client.CreateDateTime,
                DefaultAcrValues = client.DefaultAcrValues,
                DefaultMaxAge = client.DefaultMaxAge,
                GrantTypes = client.GrantTypes == null ? new List<string>() : client.GrantTypes.ToList(),
                IdTokenEncryptedResponseAlg = client.IdTokenEncryptedResponseAlg,
                IdTokenEncryptedResponseEnc = client.IdTokenEncryptedResponseEnc,
                IdTokenSignedResponseAlg = client.IdTokenSignedResponseAlg,
                InitiateLoginUri = client.InitiateLoginUri,
                JsonWebKeys = client.JsonWebKeys == null ? new List<JsonWebKey>() : client.JsonWebKeys.Select(j => j.Copy()).ToList(),
                JwksUri = client.JwksUri,
                PostLogoutRedirectUris = client.PostLogoutRedirectUris == null ? new List<string>() : client.PostLogoutRedirectUris.ToList(),
                RedirectionUrls = client.RedirectionUrls == null ? new List<string>() : client.RedirectionUrls.ToList(),
                RequestObjectEncryptionAlg = client.RequestObjectEncryptionAlg,
                RequestObjectEncryptionEnc = client.RequestObjectEncryptionEnc,
                RequestObjectSigningAlg = client.RequestObjectSigningAlg,
                RequestUris = client.RequestUris == null ? new List<string>() : client.RequestUris.ToList(),
                RequireAuthTime = client.RequireAuthTime,
                RequirePkce = client.RequirePkce,
                ScimProfile = client.ScimProfile,
                Secrets = client.Secrets == null ? new List<ClientSecret>() : client.Secrets.Select(s => s.Copy()).ToList(),
                SectorIdentifierUri = client.SectorIdentifierUri,
                SubjectType = client.SubjectType,
                TokenEndPointAuthMethod = client.TokenEndPointAuthMethod,
                TokenEndPointAuthSigningAlg = client.TokenEndPointAuthSigningAlg,
                UpdateDateTime = client.UpdateDateTime,
                UserInfoEncryptedResponseAlg = client.UserInfoEncryptedResponseAlg,
                UserInfoEncryptedResponseEnc = client.UserInfoEncryptedResponseEnc,
                UserInfoSignedResponseAlg = client.UserInfoSignedResponseAlg
            };
        }

        public static ClientSecret Copy(this ClientSecret clientSecret)
        {
            return new ClientSecret(clientSecret.Type, clientSecret.Value);
        }

        public static OAuthScope Copy(this OAuthScope scope)
        {
            return new OAuthScope
            {
                CreateDateTime = scope.CreateDateTime,
                Descriptions = scope.Descriptions.Select(d => (OAuthTranslation)d.Clone()).ToList(),
                IsDisplayedInConsent = scope.IsDisplayedInConsent,
                IsExposed = scope.IsExposed,
                IsOpenIdScope = scope.IsOpenIdScope,
                Name = scope.Name,
                Type = scope.Type,
                UpdateDateTime = scope.UpdateDateTime,
                Claims = scope.Claims == null ? new List<string>() : scope.Claims.ToList()
            };
        }
    }
}
