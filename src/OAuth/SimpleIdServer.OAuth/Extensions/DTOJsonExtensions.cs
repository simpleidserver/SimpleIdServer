// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.DTOs;
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
                .AddNotEmpty(OAuthClientParameters.SoftwareVersion, client.SoftwareVersion);
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
    }
}
