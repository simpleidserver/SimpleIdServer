// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OpenID.Domains;
using System;
using System.Collections.Generic;

namespace $rootnamespace$
{
    public class DefaultConfiguration
    {
        public static List<AuthenticationContextClassReference> AcrLst => new List<AuthenticationContextClassReference>
        {
            new AuthenticationContextClassReference
            {
                DisplayName = "First level of assurance",
                Name = "sid-load-01",
                AuthenticationMethodReferences = new List<string>
                {
                    "pwd"
                }
            },
            new AuthenticationContextClassReference
            {
                DisplayName = "Second level of assurance",
                Name = "sid-load-02",
                AuthenticationMethodReferences = new List<string>
                {
                    "pwd",
                    "sms"
                }
            }
        };

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
                    { SimpleIdServer.Jwt.Constants.UserClaims.Name, "Administrator" },
                    { SimpleIdServer.Jwt.Constants.UserClaims.Email, "administrator@hotmail.fr" },
                    { SimpleIdServer.Jwt.Constants.UserClaims.PhoneNumber, "01" }
                }
            },
            new OAuthUser
            {
                Id = "thabart",
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
                    { SimpleIdServer.Jwt.Constants.UserClaims.Subject, "thabart" },
                    { SimpleIdServer.Jwt.Constants.UserClaims.Name, "Thierry Habart" },
                    { SimpleIdServer.Jwt.Constants.UserClaims.Email, "habarthierry@hotmail.fr" },
                    { SimpleIdServer.Jwt.Constants.UserClaims.PhoneNumber, "02" }
                }
            }
        };

        public static List<OpenIdClient> Clients => new List<OpenIdClient>
        {
            new OpenIdClient
            {
                ClientId = "f3d35cce-de69-45bf-958c-4a8796f8ed37",
                Secrets = new List<ClientSecret>
                {
                    new ClientSecret(ClientSecretTypes.SharedSecret, PasswordHelper.ComputeHash("BankCvSecret"))
                },
                ClientNames = new []
                {
                    new OAuthTranslation("f3d35cce-de69-45bf-958c-4a8796f8ed37_client_name", "BankCV site", "fr"),
                    new OAuthTranslation("f3d35cce-de69-45bf-958c-4a8796f8ed37_client_name", "BankCV website", "en")
                },
                TokenEndPointAuthMethod = "client_secret_post",
                ApplicationType = "web",
                UpdateDateTime = DateTime.UtcNow,
                CreateDateTime = DateTime.UtcNow,
                TokenExpirationTimeInSeconds = 60 * 30,
                RefreshTokenExpirationTimeInSeconds = 60 * 30,
                AllowedScopes = new List<OpenIdScope>
                {
                    new OpenIdScope
                    {
                        Name = SimpleIdServer.OpenID.SIDOpenIdConstants.StandardScopes.Profile.Name
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
                PreferredTokenProfile = "Bearer",
                ResponseTypes = new List<string>
                {
                    "code"
                }
            }
        };
    }
}