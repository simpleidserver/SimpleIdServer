// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.OAuth.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.OAuth.Extensions
{
    public static class DTOJsonExtensions
    {
        public static JObject ToDto(this OAuthScope scope)
        {
            return new JObject
            {
                { OAuthScopeParameters.Name, scope.Name },
                { OAuthScopeParameters.IsExposed, scope.IsExposedInConfigurationEdp },
                { OAuthScopeParameters.IsStandard, scope.IsStandardScope },
                { OAuthScopeParameters.UpdateDateTime, scope.UpdateDateTime },
                { OAuthScopeParameters.CreateDateTime, scope.CreateDateTime },
                { OAuthScopeParameters.Claims, new JArray(scope.Claims.Select(c => c.ClaimName).ToArray()) }
            };
        }

        public static OAuthClient ToDomain(this JObject jObj)
        {
            var result = new OAuthClient();
            jObj.EnrichDomain(result);
            return result;
        }

        public static OAuthScope ToScopeDomain(this JObject jObj)
        {
            return new OAuthScope
            {
                Name = jObj.GetScopeName(),
                Claims = jObj.GetClaims().Select(c => new OAuthScopeClaim(c, false)).ToList()
            };
        }

        public static void EnrichDomain(this JObject jObj, BaseClient result)
        {
            result.ClientId = jObj.GetClientId();
            result.TokenEndPointAuthMethod = jObj.GetTokenEndpointAuthMethod();
            result.GrantTypes = jObj.GetGrantTypes();
            result.ResponseTypes = jObj.GetResponseTypes();
            result.Contacts = jObj.GetContacts();
            result.JwksUri = jObj.GetJwksUri();
            result.JsonWebKeys = jObj.GetJwks();
            result.SoftwareId = jObj.GetSoftwareId();
            result.SoftwareVersion = jObj.GetSoftwareVersion();
            result.RedirectionUrls = jObj.GetRedirectUris();
            result.AllowedScopes = jObj.GetScopes().Select(_ => new OAuthScope
            {
                Name = _
            }).ToList();
            result.TokenSignedResponseAlg = jObj.GetTokenSignedResponseAlg();
            result.TokenEncryptedResponseAlg = jObj.GetTokenEncryptedResponseAlg();
            result.TokenEncryptedResponseEnc = jObj.GetTokenEncryptedResponseEnc();
            result.RegistrationAccessToken = jObj.GetRegistrationAccessToken();
            result.TlsClientAuthSubjectDN = jObj.GetTlsClientAuthSubjectDn();
            result.TlsClientAuthSanDNS = jObj.GetTlsClientAuthSanDNS();
            result.TlsClientAuthSanURI = jObj.GetTlsClientAuthSanUri();
            result.TlsClientAuthSanIP = jObj.GetTlsClientAuthSanIP();
            result.TlsClientAuthSanEmail = jObj.GetTlsClientAuthSanEmail();
            result.ClientSecret = jObj.GetClientSecret();
            var refreshTokenExpirationTimeInSeconds = jObj.GetRefreshTokenExpirationTimeInSeconds();
            var tokenExpirationTimeInSeconds = jObj.GetTokenExpirationTimeInSeconds();
            if (refreshTokenExpirationTimeInSeconds != null)
            {
                result.RefreshTokenExpirationTimeInSeconds = refreshTokenExpirationTimeInSeconds.Value;
            }

            if (tokenExpirationTimeInSeconds != null)
            {
                result.TokenExpirationTimeInSeconds = tokenExpirationTimeInSeconds.Value;
            }

            var clientSecret = jObj.GetClientSecret();
            Dictionary<string, string> clientNames = jObj.GetClientNames(),
                clientUris = jObj.GetClientUris(),
                logoUris = jObj.GetLogoUris(),
                tosUris = jObj.GetTosUris(),
                policyUris = jObj.GetPolicyUris();
            foreach (var kvp in clientNames)
            {
                result.AddClientName(kvp.Key, kvp.Value);
            }

            foreach (var kvp in clientUris)
            {
                result.AddClientUri(kvp.Key, kvp.Value);
            }

            foreach (var kvp in logoUris)
            {
                result.AddLogoUri(kvp.Key, kvp.Value);
            }

            foreach (var kvp in tosUris)
            {
                result.AddTosUri(kvp.Key, kvp.Value);
            }

            foreach (var kvp in policyUris)
            {
                result.AddPolicyUri(kvp.Key, kvp.Value);
            }
        }
    }
}
