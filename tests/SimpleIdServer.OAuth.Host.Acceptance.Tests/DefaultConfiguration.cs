// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Helpers;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace SimpleIdServer.OAuth.Host.Acceptance.Tests
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
                Claims = new List<Claim>
                {
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.Subject, "administrator"),
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.Name, "Thierry Habart"),
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.Email, "habarthierry@hotmail.fr")
                }
            }
        };

        public static List<OAuthScope> Scopes => new List<OAuthScope>
        {
            new OAuthScope
            {
                Name = "scope1",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "role",
                IsExposedInConfigurationEdp = true
            }
        };

        public static List<OAuthClient> Clients => new List<OAuthClient>
        {
            new OAuthClient
            {
                ClientId = "f3d35cce-de69-45bf-958c-4a8796f8ed37",
                Secrets = new List<ClientSecret>
                {
                    new ClientSecret(ClientSecretTypes.SharedSecret, "BankCvSecret", null)
                },
                ClientNames = new []
                {
                    new OAuthTranslation("f3d35cce-de69-45bf-958c-4a8796f8ed37_client_name", "BankCV website", string.Empty)
                },
                TokenEndPointAuthMethod = "client_secret_post",
                UpdateDateTime = DateTime.UtcNow,
                CreateDateTime = DateTime.UtcNow,
                TokenExpirationTimeInSeconds = 60 * 30,
                RefreshTokenExpirationTimeInSeconds = 60 * 30,
                AllowedScopes = new List<OAuthScope>
                {
                    new OAuthScope
                    {
                        Name = "scope1"
                    }
                },
                GrantTypes = new List<string>
                {
                    "client_credentials",
                    "refresh_token",
                    "authorization_code",
                    "password",
                    "refresh_token"
                },
                ResponseTypes = new List<string>
                {
                    "code"
                },
                RedirectionUrls = new List<string>
                {
                    "http://localhost:8080",
                    "http://localhost:1700"
                },
                PreferredTokenProfile = "Bearer",
                TokenSignedResponseAlg = "RS256"
            }
        };
    }
}