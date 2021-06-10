// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Domains;
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

        public static JObject ToDto(this BaseClient client, string issuer)
        {
            var result = new JObject();
            result.AddNotEmpty(OAuthClientParameters.ClientId, client.ClientId)
                .AddNotEmpty(OAuthClientParameters.RegistrationAccessToken, client.RegistrationAccessToken)
                .AddNotEmpty(OAuthClientParameters.GrantTypes, client.GrantTypes)
                .AddNotEmpty(OAuthClientParameters.RedirectUris, client.RedirectionUrls)
                .AddNotEmpty(OAuthClientParameters.TokenEndpointAuthMethod, client.TokenEndPointAuthMethod)
                .AddNotEmpty(OAuthClientParameters.ResponseTypes, client.ResponseTypes)
                .AddNotEmpty(OAuthClientParameters.ClientName, client.ClientNames)
                .AddNotEmpty(OAuthClientParameters.ClientUri, client.ClientUris)
                .AddNotEmpty(OAuthClientParameters.LogoUri, client.LogoUris)
                .AddNotEmpty(OAuthClientParameters.Contacts, client.Contacts)
                .AddNotEmpty(OAuthClientParameters.TosUri, client.TosUris)
                .AddNotEmpty(OAuthClientParameters.PolicyUri, client.PolicyUris)
                .AddNotEmpty(OAuthClientParameters.JwksUri, client.JwksUri)
                .AddNotEmpty(OAuthClientParameters.SoftwareId, client.SoftwareId)
                .AddNotEmpty(OAuthClientParameters.SoftwareVersion, client.SoftwareVersion)
                .AddNotEmpty(OAuthClientParameters.TlsClientAuthSubjectDN, client.TlsClientAuthSubjectDN)
                .AddNotEmpty(OAuthClientParameters.TlsClientAuthSanDNS, client.TlsClientAuthSanDNS)
                .AddNotEmpty(OAuthClientParameters.TlsClientAuthSanUri, client.TlsClientAuthSanURI)
                .AddNotEmpty(OAuthClientParameters.TlsClientAuthSanIp, client.TlsClientAuthSanIP)
                .AddNotEmpty(OAuthClientParameters.TlsClientAuthSanEmail, client.TlsClientAuthSanEmail)
                .AddNotEmpty(OAuthClientParameters.ClientSecret, client.ClientSecret)
                .AddNotEmpty(OAuthClientParameters.ClientSecretExpiresAt, client.ClientSecretExpirationTime);
            result.Add(OAuthClientParameters.UpdateDateTime, client.UpdateDateTime);
            result.Add(OAuthClientParameters.CreateDateTime, client.CreateDateTime);
            result.Add(OAuthClientParameters.ClientIdIssuedAt, client.CreateDateTime.ConvertToUnixTimestamp());
            result.Add(OAuthClientParameters.RegistrationClientUri, $"{issuer}/{Constants.EndPoints.Registration}/{client.ClientId}");
            result.Add(OAuthClientParameters.TokenExpirationTimeInSeconds, client.TokenExpirationTimeInSeconds);
            result.Add(OAuthClientParameters.RefreshTokenExpirationTimeInSeconds, client.RefreshTokenExpirationTimeInSeconds);
            if (client.AllowedScopes != null && client.AllowedScopes.Any())
            {
                result.Add(OAuthClientParameters.Scope, string.Join(" ", client.AllowedScopes.Select(_ => _.Name)));
            }

            if (client.JsonWebKeys != null && client.JsonWebKeys.Any())
            {
                var keys = new JObject
                {
                    { "keys", JArray.FromObject(client.JsonWebKeys.Select(_ => _.Serialize())) }
                };
                result.Add(OAuthClientParameters.Jwks, keys);
            }

            return result;
        }

        public static OAuthClient ToDomain(this JObject jObj)
        {
            var result = new OAuthClient();
            jObj.EnrichDomain(result);
            return result;
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
