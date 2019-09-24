// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Domains.Clients;
using SimpleIdServer.OAuth.Domains.Scopes;
using SimpleIdServer.OAuth.Domains.Users;
using SimpleIdServer.OAuth.Helpers;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.OAuth.Startup
{
    public class DefaultConfiguration
    {
        public static List<OAuthUser> Users => new List<OAuthUser>
        {
            new OAuthUser
            {
                Id = "administrator",
                Credentials = new List<OAuthUserCredential>
                {
                    new OAuthUserCredential
                    {
                        CredentialType = "pwd",
                        Value = PasswordHelper.ComputeHash("password")
                    }
                },
                Claims = new Dictionary<string, string>
                {
                    { SimpleIdServer.Jwt.Constants.UserClaims.Subject, "administrator" },
                    { SimpleIdServer.Jwt.Constants.UserClaims.Name, "Thierry Habart" },
                    { SimpleIdServer.Jwt.Constants.UserClaims.Email, "habarthierry@hotmail.fr" }
                }
            }
        };

        public static List<OAuthScope> Scopes => new List<OAuthScope>
        {
            new OAuthScope
            {
                Name = "scope1",
               Descriptions = new List<OAuthTranslation>
               {
                   new OAuthTranslation("scope1_description", "Access to the scope1", "en"),
                   new OAuthTranslation("scope1_description", "Accéder au scope1", "fr")
               },
                IsExposedInConfigurationEdp = true
            }
        };

        public static List<OAuthClient> Clients => new List<OAuthClient>
        {
            new OAuthClient
            {
                ClientId = "",
                Secrets = new List<ClientSecret>
                {
                    new ClientSecret(ClientSecretTypes.SharedSecret, "BankCvSecret")
                },
                ClientNames = new []
                {
                    new OAuthTranslation("f3d35cce-de69-45bf-958c-4a8796f8ed37_client_name", "BankCV website", string.Empty)
                },
                TokenEndPointAuthMethod = "client_secret_post",
                ApplicationType = "web",
                UpdateDateTime = DateTime.UtcNow,
                CreateDateTime = DateTime.UtcNow,
                TokenExpirationTimeInSeconds = 60 * 30,
                RefreshTokenExpirationTimeInSeconds = 60 * 30,
                AllowedScopes = new List<OAuthScope>
                {
                    new OAuthScope
                    {
                        Name = "scope1",
                        Descriptions = new List<OAuthTranslation>
                        {
                            new OAuthTranslation("scope1_description", "Access to the scope1", "en"),
                            new OAuthTranslation("scope1_description", "Accéder au scope1", "fr")
                        }
                    }
                },
                GrantTypes = new List<string>
                {
                    "client_credentials",
                    "refresh_token",
                    "authorization_code"
                },
                RedirectionUrls = new List<string>
                {
                    "http://localhost:8080",
                    "http://localhost:1700"
                },
                PostLogoutRedirectUris = new List<string>
                {
                    "http://localhost:8080",
                    "http://localhost:1700"
                },
                PreferredTokenProfile = "Bearer"
            }
        };
    }
}