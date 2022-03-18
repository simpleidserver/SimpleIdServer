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
        public static OAuthScope AdminScope = new OAuthScope("admin");
        public static OAuthScope FirstOrgScope = new OAuthScope("firstOrg");
        public static OAuthScope SecondOrgScope = new OAuthScope("secondOrg");
        public static List<OAuthScope> Scopes = new List<OAuthScope>
        {
            SIDOpenIdConstants.StandardScopes.OpenIdScope,
            SIDOpenIdConstants.StandardScopes.Phone,
            SIDOpenIdConstants.StandardScopes.Profile,
            SIDOpenIdConstants.StandardScopes.Role,
            SIDOpenIdConstants.StandardScopes.OfflineAccessScope,
            SIDOpenIdConstants.StandardScopes.Email,
            SIDOpenIdConstants.StandardScopes.Address,            
            SIDOpenIdConstants.StandardScopes.ScimScope,
            AdminScope,
            FirstOrgScope,
            SecondOrgScope
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
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.Role, "admin"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.Email, "agentsimpleidserver@gmail.com"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.UpdatedAt, "1612355959", SimpleIdServer.Jwt.ClaimValueTypes.INTEGER),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.EmailVerified, "true", SimpleIdServer.Jwt.ClaimValueTypes.BOOLEAN),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.Address, "{ 'street_address': '1234 Hollywood Blvd.', 'locality': 'Los Angeles', 'region': 'CA', 'postal_code': '90210', 'country': 'US' }", SimpleIdServer.Jwt.ClaimValueTypes.JSONOBJECT),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.PhoneNumber, "+1 (310) 123-4567"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.PhoneNumberVerified, "true", SimpleIdServer.Jwt.ClaimValueTypes.BOOLEAN)
                }
            },
            new OAuthUser
            {
                Id = "firstUser",
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
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.Subject, "firstUser"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.Name, "name"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.FamilyName, "familyName"),
                    new UserClaim("org", "firstOrg")
                }
            },
            new OAuthUser
            {
                Id = "secondUser",
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
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.Subject, "secondUser"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.Name, "name"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.FamilyName, "familyName"),
                    new UserClaim("org", "secondOrg")
                }
            }
        };

        public static List<OpenIdClient> GetClients()
        {
            return new List<OpenIdClient>
            {
                new OpenIdClient
                {
                    ClientId = "admin",
                    ClientSecret = "secret",
                    ApplicationKind = ApplicationKinds.Service,
                    TokenEndPointAuthMethod = "client_secret_post",
                    UpdateDateTime = DateTime.UtcNow,
                    CreateDateTime = DateTime.UtcNow,
                    TokenExpirationTimeInSeconds = 60 * 30,
                    RefreshTokenExpirationTimeInSeconds = 60 * 30,
                    TokenSignedResponseAlg = "RS256",
                    AllowedScopes = new List<OAuthScope>
                    {
                        AdminScope
                    },
                    GrantTypes = new List<string>
                    {
                        "client_credentials"
                    },
                    PreferredTokenProfile = "Bearer"
                },
                new OpenIdClient
                {
                    ClientId = "firstOrg",
                    ClientSecret = "secret",
                    ApplicationKind = ApplicationKinds.Service,
                    TokenEndPointAuthMethod = "client_secret_post",
                    UpdateDateTime = DateTime.UtcNow,
                    CreateDateTime = DateTime.UtcNow,
                    TokenExpirationTimeInSeconds = 60 * 30,
                    RefreshTokenExpirationTimeInSeconds = 60 * 30,
                    TokenSignedResponseAlg = "RS256",
                    AllowedScopes = new List<OAuthScope>
                    {
                        FirstOrgScope
                    },
                    GrantTypes = new List<string>
                    {
                        "client_credentials"
                    },
                    PreferredTokenProfile = "Bearer"
                },
                new OpenIdClient
                {
                    ClientId = "secondOrg",
                    ClientSecret = "secret",
                    ApplicationKind = ApplicationKinds.Service,
                    TokenEndPointAuthMethod = "client_secret_post",
                    UpdateDateTime = DateTime.UtcNow,
                    CreateDateTime = DateTime.UtcNow,
                    TokenExpirationTimeInSeconds = 60 * 30,
                    RefreshTokenExpirationTimeInSeconds = 60 * 30,
                    TokenSignedResponseAlg = "RS256",
                    AllowedScopes = new List<OAuthScope>
                    {
                        SecondOrgScope
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
}