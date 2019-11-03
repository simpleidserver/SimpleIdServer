// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Helpers;
using System;
using System.Collections.Generic;

namespace ProtectAPIFromUndesirableClients.OAuth
{
    public class DefaultConfiguration
    {
        public static List<OAuthScope> Scopes => new List<OAuthScope>
        {
            new OAuthScope
            {
                CreateDateTime = DateTime.UtcNow,
                IsExposedInConfigurationEdp = true,
                Name = "get_user",
                UpdateDateTime = DateTime.UtcNow
            },
            new OAuthScope
            {
                CreateDateTime = DateTime.UtcNow,
                IsExposedInConfigurationEdp = true,
                Name = "add_user",
                UpdateDateTime = DateTime.UtcNow
            }
        };

        public static List<OAuthClient> Clients => new List<OAuthClient>
        {
            new OAuthClient
            {
                ClientId = "application",
                Secrets = new List<ClientSecret>
                {
                    new ClientSecret(ClientSecretTypes.SharedSecret, PasswordHelper.ComputeHash("applicationSecret"))
                },
                TokenEndPointAuthMethod = "client_secret_post",
                UpdateDateTime = DateTime.UtcNow,
                CreateDateTime = DateTime.UtcNow,
                TokenExpirationTimeInSeconds = 60 * 30,
                RefreshTokenExpirationTimeInSeconds = 60 * 30,
                TokenSignedResponseAlg = "RS256",
                AllowedScopes = new List<OAuthScope>
                {
                    new OAuthScope
                    {
                        CreateDateTime = DateTime.UtcNow,
                        IsExposedInConfigurationEdp = true,
                        Name = "get_user",
                        UpdateDateTime = DateTime.UtcNow
                    },
                    new OAuthScope
                    {
                        CreateDateTime = DateTime.UtcNow,
                        IsExposedInConfigurationEdp = true,
                        Name = "add_user",
                        UpdateDateTime = DateTime.UtcNow
                    }
                },
                GrantTypes = new List<string>
                {
                    "client_credentials"
                },
                PreferredTokenProfile = "Bearer",
                ResponseTypes = new List<string>
                {
                    "token"
                }
            }
        };
    }
}