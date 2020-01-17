// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OpenID;
using SimpleIdServer.OpenID.Domains;
using System;
using System.Collections.Generic;

namespace $rootnamespace$
{
    public class DefaultConfiguration
    {
        public static List<OpenIdScope> Scopes = new List<OpenIdScope>
        {
            new OpenIdScope
            {
                Name = "scim",
                Claims = new List<string>
                {
                    "scim_id"
                }
            }
        };

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
                Id = "scimUser",
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
                    { SimpleIdServer.Jwt.Constants.UserClaims.Subject, "scimUser" },
                    { "scim_id", "1" }
                }
            },
            new OAuthUser
            {
                Id = "umaUser",
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
                    { SimpleIdServer.Jwt.Constants.UserClaims.Subject, "umaUser" },
                    { SimpleIdServer.Jwt.Constants.UserClaims.Name, "User" },
                    { SimpleIdServer.Jwt.Constants.UserClaims.UniqueName, "User" }
                }
            }
        };

        public static List<OpenIdClient> Clients => new List<OpenIdClient>
        {
            new OpenIdClient
            {
                ClientId = "scimClient",
                Secrets = new List<ClientSecret>
                {
                    new ClientSecret(ClientSecretTypes.SharedSecret, PasswordHelper.ComputeHash("scimClientSecret"))
                },
                TokenEndPointAuthMethod = "client_secret_post",
                ApplicationType = "web",
                UpdateDateTime = DateTime.UtcNow,
                CreateDateTime = DateTime.UtcNow,
                TokenExpirationTimeInSeconds = 60 * 30,
                RefreshTokenExpirationTimeInSeconds = 60 * 30,
                TokenSignedResponseAlg = "RS256",
                IdTokenSignedResponseAlg = "RS256",
                AllowedScopes = new List<OpenIdScope>
                {
                    new OpenIdScope
                    {
                        Name = "scim",
                        Claims = new List<string>
                        {
                            "scim_id"
                        }
                    }
                },
                GrantTypes = new List<string>
                {
                    "implicit",
                },
                RedirectionUrls = new List<string>
                {
                    "http://localhost:8080",
                    "http://localhost:1700"
                },
                PreferredTokenProfile = "Bearer",
                ResponseTypes = new List<string>
                {
                    "token",
                    "id_token"
                }
            },
            new OpenIdClient
            {
                ClientId = "umaClient",
                Secrets = new List<ClientSecret>
                {
                    new ClientSecret(ClientSecretTypes.SharedSecret, PasswordHelper.ComputeHash("umaClientSecret"))
                },
                TokenEndPointAuthMethod = "client_secret_post",
                ApplicationType = "web",
                UpdateDateTime = DateTime.UtcNow,
                CreateDateTime = DateTime.UtcNow,
                TokenExpirationTimeInSeconds = 60 * 30,
                RefreshTokenExpirationTimeInSeconds = 60 * 30,
                TokenSignedResponseAlg = "RS256",
                IdTokenSignedResponseAlg = "RS256",
                AllowedScopes = new List<OpenIdScope>
                {
                    SIDOpenIdConstants.StandardScopes.Profile,
                    SIDOpenIdConstants.StandardScopes.Email
                },
                GrantTypes = new List<string>
                {
                    "implicit",
                    "authorization_code"
                },
                RedirectionUrls = new List<string>
                {
                    "https://localhost:60001/signin-oidc"
                },
                PreferredTokenProfile = "Bearer",
                ResponseTypes = new List<string>
                {
                    "token",
                    "id_token",
                    "code"
                }
            }
        };
    }
}