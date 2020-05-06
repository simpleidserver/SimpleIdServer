// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.DTOs;
using System;
using System.Linq;

namespace SimpleIdServer.OAuth.Extensions
{
    public static class DTOJsonExtensions
    {
        public static JObject ToDto(this OAuthClient client)
        {
            var result = new JObject
            {
                { "client_id", client.ClientId },
                { "create_datetime", client.CreateDateTime },
                { "update_datetime", client.UpdateDateTime },
                { "preferred_token_profile", client.PreferredTokenProfile },
                { RegisterRequestParameters.TokenEndpointAuthMethod, client.TokenEndPointAuthMethod },
                { RegisterRequestParameters.JwksUri, client.JwksUri },
                { RegisterRequestParameters.SoftwareId, client.SoftwareId },
                { RegisterRequestParameters.SoftwareVersion, client.SoftwareVersion },
                { RegisterRequestParameters.TokenSignedResponseAlg, client.TokenSignedResponseAlg },
                { RegisterRequestParameters.TokenEncryptedResponseAlg, client.TokenEncryptedResponseAlg },
                { RegisterRequestParameters.TokenEncryptedResponseEnc, client.TokenEncryptedResponseEnc },
                { RegisterRequestParameters.TokenExpirationTimeInSeconds, client.TokenExpirationTimeInSeconds },
                { RegisterRequestParameters.RefreshTokenExpirationTimeInSeconds, client.RefreshTokenExpirationTimeInSeconds }
            };
            if (client.Secrets != null)
            {
                result.Add("secrets", new JArray(client.Secrets.Select(_ => _.ToDto())));
            }

            if (client.Contacts != null)
            {
                result.Add(RegisterRequestParameters.Contacts, new JArray(client.Contacts));
            }

            if (client.PolicyUris != null)
            {
                result.Add(RegisterRequestParameters.PolicyUri, new JArray(client.PolicyUris.Select(_ => _.ToDto())));
            }

            if (client.RedirectionUrls != null)
            {
                result.Add(RegisterRequestParameters.RedirectUris, new JArray(client.RedirectionUrls));
            }

            if (client.GrantTypes != null)
            {
                result.Add(RegisterRequestParameters.GrantTypes, new JArray(client.GrantTypes));
            }

            if (client.ResponseTypes != null)
            {
                result.Add(RegisterRequestParameters.ResponseTypes, new JArray(client.ResponseTypes));
            }

            if (client.ClientNames != null)
            {
                result.Add(RegisterRequestParameters.ClientName, new JArray(client.ClientNames.Select(_ => ToDto(_))));
            }

            if (client.ClientUris != null)
            {
                result.Add(RegisterRequestParameters.ClientUri, new JArray(client.ClientUris.Select(_ => ToDto(_))));
            }

            if (client.LogoUris != null)
            {
                result.Add(RegisterRequestParameters.LogoUri, new JArray(client.LogoUris.Select(_ => ToDto(_))));
            }

            if (client.AllowedScopes != null)
            {
                result.Add(RegisterRequestParameters.Scope, new JArray(client.AllowedScopes.Select(_ => _.Name)));
            }

            if (client.TosUris != null)
            {
                result.Add(RegisterRequestParameters.TosUri, new JArray(client.TosUris.Select(_ => ToDto(_))));
            }

            return result;
        }

        public static JObject ToDto(this ClientSecret clientSecret)
        {
            var result = new JObject
            {
                { "type", Enum.GetName(typeof(ClientSecretTypes), clientSecret.Type).ToLowerInvariant() },
                { "value", clientSecret.Value }
            };
            return result;
        }

        public static JObject ToDto(this OAuthTranslation translation)
        {
            return new JObject
            {
                { "language", translation.Language },
                { "value", translation.Value }
            };
        }
    }
}
