// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Common.Domains;
using SimpleIdServer.Common.Helpers;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OpenID;
using SimpleIdServer.OpenID.Domains;
using System;
using System.Collections.Generic;

namespace OpenId
{
    public class OpenIdDefaultConfiguration
    {
        public static List<OAuthScope> Scopes = new List<OAuthScope>
        {
            SIDOpenIdConstants.StandardScopes.OpenIdScope,
            SIDOpenIdConstants.StandardScopes.Phone,
            SIDOpenIdConstants.StandardScopes.Profile,
            SIDOpenIdConstants.StandardScopes.Role,
            SIDOpenIdConstants.StandardScopes.OfflineAccessScope,
            SIDOpenIdConstants.StandardScopes.Email,
            SIDOpenIdConstants.StandardScopes.Address,            
            SIDOpenIdConstants.StandardScopes.ScimScope
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
                Id = "sub",
                Credentials = new List<UserCredential>
                {
                    new UserCredential
                    {
                        CredentialType = "pwd",
                        Value = PasswordHelper.ComputeHash("password")
                    }
                },
                CreateDateTime = DateTime.Now,
                UpdateDateTime = DateTime.Now,
                OAuthUserClaims = new List<UserClaim>
                {
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.Subject, "sub"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.Name, "name"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.FamilyName, "familyName"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.UniqueName, "uniquename"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.GivenName, "givenName"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.MiddleName, "middleName"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.NickName, "nickName"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.BirthDate, "07-10-1989"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.PreferredUserName, "preferredUserName"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.ZoneInfo, "zoneInfo"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.Locale, "locale"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.Picture, "picture"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.WebSite, "website"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.Profile, "profile"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.Gender, "gender"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.Email, "agentsimpleidserver@gmail.com"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.UpdatedAt, "1612355959", SimpleIdServer.Jwt.ClaimValueTypes.INTEGER),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.EmailVerified, "true", SimpleIdServer.Jwt.ClaimValueTypes.BOOLEAN),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.Address, "{ 'street_address': '1234 Hollywood Blvd.', 'locality': 'Los Angeles', 'region': 'CA', 'postal_code': '90210', 'country': 'US' }", SimpleIdServer.Jwt.ClaimValueTypes.JSONOBJECT),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.PhoneNumber, "+1 (310) 123-4567"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.PhoneNumberVerified, "true", SimpleIdServer.Jwt.ClaimValueTypes.BOOLEAN)
                }
            }
        };

        public static List<OpenIdClient> GetClients()
        {
            return new List<OpenIdClient>
            {
                new OpenIdClient
                {
                    ClientId = "scimClient",
                    ClientSecret = "scimClientSecret",
                    ApplicationKind = ApplicationKinds.Service,
                    TokenEndPointAuthMethod = "client_secret_post",
                    ApplicationType = "web",
                    UpdateDateTime = DateTime.UtcNow,
                    CreateDateTime = DateTime.UtcNow,
                    TokenExpirationTimeInSeconds = 60 * 30,
                    RefreshTokenExpirationTimeInSeconds = 60 * 30,
                    TokenSignedResponseAlg = "RS256",
                    IdTokenSignedResponseAlg = "RS256",
                    AllowedScopes = new List<OAuthScope>
                    {
                        SIDOpenIdConstants.StandardScopes.ScimScope
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
                    ClientSecret = "umaClientSecret",
                    ApplicationKind = ApplicationKinds.Service,
                    TokenEndPointAuthMethod = "client_secret_post",
                    ApplicationType = "web",
                    UpdateDateTime = DateTime.UtcNow,
                    CreateDateTime = DateTime.UtcNow,
                    TokenExpirationTimeInSeconds = 60 * 30,
                    RefreshTokenExpirationTimeInSeconds = 60 * 30,
                    TokenSignedResponseAlg = "RS256",
                    IdTokenSignedResponseAlg = "RS256",
                    AllowedScopes = new List<OAuthScope>
                    {
                        SIDOpenIdConstants.StandardScopes.OpenIdScope,
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
                },
                new OpenIdClient
                {
                    ClientId = "simpleIdServerWebsite",
                    ClientSecret = "simpleIdServerWebsiteSecret",
                    ApplicationKind = ApplicationKinds.SPA,
                    TokenEndPointAuthMethod = "pkce",
                    ApplicationType = "web",
                    UpdateDateTime = DateTime.UtcNow,
                    CreateDateTime = DateTime.UtcNow,
                    TokenExpirationTimeInSeconds = 60 * 30,
                    RefreshTokenExpirationTimeInSeconds = 60 * 30,
                    TokenSignedResponseAlg = "RS256",
                    IdTokenSignedResponseAlg = "RS256",
                    AllowedScopes = new List<OAuthScope>
                    {
                        SIDOpenIdConstants.StandardScopes.OpenIdScope,
                        SIDOpenIdConstants.StandardScopes.Profile,
                        SIDOpenIdConstants.StandardScopes.Email,
                        SIDOpenIdConstants.StandardScopes.Role
                    },
                    GrantTypes = new List<string>
                    {
                        "authorization_code",
                        "implicit"
                    },
                    RedirectionUrls = new List<string>
                    {
                        "http://localhost:4200",
                        "https://simpleidserver.northeurope.cloudapp.azure.com/simpleidserver/"
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        "http://localhost:4200",
                        "https://simpleidserver.northeurope.cloudapp.azure.com/simpleidserver/"
                    },
                    PreferredTokenProfile = "Bearer",
                    ResponseTypes = new List<string>
                    {
                        "token",
                        "id_token",
                        "code"
                    }
                },
                new OpenIdClient
                {
                    ClientId = "tradWebsite",
                    ClientSecret = "tradWebsiteSecret",
                    ApplicationKind = ApplicationKinds.Web,
                    TokenEndPointAuthMethod = "client_secret_post",
                    ApplicationType = "web",
                    UpdateDateTime = DateTime.UtcNow,
                    CreateDateTime = DateTime.UtcNow,
                    TokenExpirationTimeInSeconds = 60 * 30,
                    RefreshTokenExpirationTimeInSeconds = 60 * 30,
                    TokenSignedResponseAlg = "RS256",
                    IdTokenSignedResponseAlg = "RS256",
                    AllowedScopes = new List<OAuthScope>
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
                    ClientId = "native",
                    ClientSecret = "nativeSecret",
                    ApplicationKind = ApplicationKinds.Native,
                    TokenEndPointAuthMethod = "pkce",
                    ApplicationType = "web",
                    UpdateDateTime = DateTime.UtcNow,
                    CreateDateTime = DateTime.UtcNow,
                    TokenExpirationTimeInSeconds = 60 * 30,
                    RefreshTokenExpirationTimeInSeconds = 60 * 30,
                    TokenSignedResponseAlg = "RS256",
                    IdTokenSignedResponseAlg = "RS256",
                    AllowedScopes = new List<OAuthScope>
                    {
                        SIDOpenIdConstants.StandardScopes.OpenIdScope,
                        SIDOpenIdConstants.StandardScopes.Profile,
                        SIDOpenIdConstants.StandardScopes.Email
                    },
                    GrantTypes = new List<string>
                    {
                        "authorization_code"
                    },
                    RedirectionUrls = new List<string>
                    {
                        "com.companyname.simpleidserver.mobileapp:/oauth2redirect"
                    },
                    PreferredTokenProfile = "Bearer",
                    ResponseTypes = new List<string>
                    {
                        "code"
                    }
                },
                new OpenIdClient
                {
                    ClientId = "website",
                    ClientSecret = "websiteSecret",
                    ApplicationKind = ApplicationKinds.Web,
                    TokenEndPointAuthMethod = "client_secret_post",
                    ApplicationType = "web",
                    UpdateDateTime = DateTime.UtcNow,
                    CreateDateTime = DateTime.UtcNow,
                    TokenExpirationTimeInSeconds = 60 * 30,
                    RefreshTokenExpirationTimeInSeconds = 60 * 30,
                    TokenSignedResponseAlg = "RS256",
                    IdTokenSignedResponseAlg = "RS256",
                    AllowedScopes = new List<OAuthScope>
                    {
                        SIDOpenIdConstants.StandardScopes.OpenIdScope,
                        SIDOpenIdConstants.StandardScopes.Profile,
                        SIDOpenIdConstants.StandardScopes.Email
                    },
                    GrantTypes = new List<string>
                    {
                        "authorization_code",
                    },
                    RedirectionUrls = new List<string>
                    {
                        "https://localhost:7000/signin-oidc"
                    },
                    PreferredTokenProfile = "Bearer",
                    ResponseTypes = new List<string>
                    {
                        "token",
                        "id_token"
                    }
                }
            };
        }
    }
}