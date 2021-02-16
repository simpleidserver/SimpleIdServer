// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.OAuth.Extensions
{
    public static class DTOJsonExtensions
    {
        public static JObject ToDto(this OAuthClient client, string issuer)
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
                .AddNotEmpty(OAuthClientParameters.TlsClientAuthSanEmail, client.TlsClientAuthSanEmail);
            result.Add(OAuthClientParameters.ClientIdIssuedAt, client.CreateDateTime.ConvertToUnixTimestamp());
            result.Add(OAuthClientParameters.RegistrationClientUri, $"{issuer}/{Constants.EndPoints.Registration}/{client.ClientId}");
            if (client.Secrets != null && client.Secrets.Any())
            {
                var secret = client.Secrets.FirstOrDefault(_ => _.Type == ClientSecretTypes.SharedSecret);
                if (secret != null)
                {
                    result.AddNotEmpty(OAuthClientParameters.ClientSecret, secret.Value);
                    result.Add(OAuthClientParameters.ClientSecretExpiresAt, secret.ExpirationDateTime == null ? 0 : secret.ExpirationDateTime.Value.ConvertToUnixTimestamp());
                }
            }

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

        public static void EnrichDomain(this JObject jObj, OAuthClient result)
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
            });
            result.TokenSignedResponseAlg = jObj.GetTokenSignedResponseAlg();
            result.TokenEncryptedResponseAlg = jObj.GetTokenEncryptedResponseAlg();
            result.TokenEncryptedResponseEnc = jObj.GetTokenEncryptedResponseEnc();
            result.RegistrationAccessToken = jObj.GetRegistrationAccessToken();
            result.TlsClientAuthSubjectDN = jObj.GetTlsClientAuthSubjectDn();
            result.TlsClientAuthSanDNS = jObj.GetTlsClientAuthSanDNS();
            result.TlsClientAuthSanURI = jObj.GetTlsClientAuthSanUri();
            result.TlsClientAuthSanIP = jObj.GetTlsClientAuthSanIP();
            result.TlsClientAuthSanEmail = jObj.GetTlsClientAuthSanEmail();
            var clientSecret = jObj.GetClientSecret();
            if (!string.IsNullOrWhiteSpace(clientSecret))
            {
                result.Secrets = new List<ClientSecret>
                {
                    new ClientSecret(ClientSecretTypes.SharedSecret, clientSecret, null)
                };
            }

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
