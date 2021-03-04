// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OpenID;
using SimpleIdServer.OpenID.Domains;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace ProtectAPIFromUndesirableUsers.OpenId
{
    public class DefaultConfiguration
    {
        public static List<OpenIdScope> Scopes = new List<OpenIdScope>
        {
            SIDOpenIdConstants.StandardScopes.OpenIdScope,
            SIDOpenIdConstants.StandardScopes.Phone,
            SIDOpenIdConstants.StandardScopes.Profile,
            SIDOpenIdConstants.StandardScopes.Role,
            SIDOpenIdConstants.StandardScopes.OfflineAccessScope,
            SIDOpenIdConstants.StandardScopes.Email,
            SIDOpenIdConstants.StandardScopes.Address,
            new OpenIdScope
            {
                Name = "scim",
                Claims = new List<OpenIdScopeClaim>
                {
                    new OpenIdScopeClaim("scim_id", true)
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
            }
        };

        public static List<OAuthUser> Users => new List<OAuthUser>
        {
            new OAuthUser
            {
                Id = "user",
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
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.Subject, "user"),
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.Name, "User"),
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.UniqueName, "User")
                }
            }
        };

        public static List<OpenIdClient> Clients => new List<OpenIdClient>
        {
            new OpenIdClient
            {
                ClientId = "tradWebsite",
                Secrets = new List<ClientSecret>
                {
                    new ClientSecret(ClientSecretTypes.SharedSecret, "tradWebsiteSecret", DateTime.UtcNow.AddDays(2))
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
                    SIDOpenIdConstants.StandardScopes.OpenIdScope,
                    SIDOpenIdConstants.StandardScopes.Profile
                },
                GrantTypes = new List<string>
                {
                    "authorization_code",
                    "password"
                },
                RedirectionUrls = new List<string>
                {
                    "https://localhost:5001/signin-oidc"
                },
                PreferredTokenProfile = "Bearer",
                ResponseTypes = new List<string>
                {
                    "code",
                    "token",
                    "id_token"
                }
            },
            new OpenIdClient
            {
                ClientId = "userAgentBased",
                Secrets = new List<ClientSecret>
                {
                    new ClientSecret(ClientSecretTypes.SharedSecret, "userAgentBasedSecret", DateTime.UtcNow.AddDays(2))
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
                    SIDOpenIdConstants.StandardScopes.OpenIdScope,
                    SIDOpenIdConstants.StandardScopes.Profile
                },
                GrantTypes = new List<string>
                {
                    "implicit"
                },
                RedirectionUrls = new List<string>
                {
                    "http://localhost:4200/index.html"
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
                ClientId = "native",
                Secrets = new List<ClientSecret>
                {
                    new ClientSecret(ClientSecretTypes.SharedSecret, "nativeSecret", DateTime.UtcNow.AddDays(2))
                },
                TokenEndPointAuthMethod = "pkce",
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
                    "authorization_code"
                },
                RedirectionUrls = new List<string>
                {
                    "sid:/oauth2redirect"
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