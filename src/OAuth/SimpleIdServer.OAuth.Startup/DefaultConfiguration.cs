// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Helpers;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.OAuth.Startup
{
    public class DefaultConfiguration
    {
        public static List<OAuthScope> Scopes => new List<OAuthScope>
        {
            new OAuthScope
            {
                Name = "query_scim_resource",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "add_scim_resource",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "delete_scim_resource",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "update_scim_resource",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "bulk_scim_resource",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "manage_clients",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "manage_scopes",
                IsExposedInConfigurationEdp = true
            }
        };

        public static List<OAuthClient> Clients => new List<OAuthClient>
        {
            new OAuthClient
            {
                ClientId = "scimClient",
                Secrets = new List<ClientSecret>
                {
                    new ClientSecret(ClientSecretTypes.SharedSecret, PasswordHelper.ComputeHash("scimClientSecret"))
                },
                ClientNames = new []
                {
                    new OAuthTranslation("scimClient_client_name", "SCIMClient", "fr")
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
                        Name = "query_scim_resource"
                    },
                    new OAuthScope
                    {
                        Name = "add_scim_resource"
                    },
                    new OAuthScope
                    {
                        Name = "delete_scim_resource"
                    },
                    new OAuthScope
                    {
                        Name = "update_scim_resource"
                    },
                    new OAuthScope
                    {
                        Name = "bulk_scim_resource"
                    }
                },
                GrantTypes = new List<string>
                {
                    "client_credentials"
                },
                PreferredTokenProfile = "Bearer"
            },
            new OAuthClient
            {
                ClientId = "gatewayClient",
                Secrets = new List<ClientSecret>
                {
                    new ClientSecret(ClientSecretTypes.SharedSecret, PasswordHelper.ComputeHash("gatewayClientPassword"))
                },
                ClientNames = new []
                {
                    new OAuthTranslation("gatewayClient_client_name", "SCIMClient", "fr")
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
                        Name = "manage_clients"
                    },
                    new OAuthScope
                    {
                        Name = "manage_scopes"
                    }
                },
                GrantTypes = new List<string>
                {
                    "client_credentials"
                },
                PreferredTokenProfile = "Bearer"
            }
        };
    }
}