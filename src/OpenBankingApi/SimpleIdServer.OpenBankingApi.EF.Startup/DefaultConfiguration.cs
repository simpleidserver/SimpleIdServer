// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OpenBankingApi.Domains.Account;
using SimpleIdServer.OpenBankingApi.Domains.Account.Enums;
using SimpleIdServer.OpenID;
using SimpleIdServer.OpenID.Domains;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Claims;

namespace SimpleIdServer.OpenBankingApi.EF.Startup
{
    public class DefaultConfiguration
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
            SIDOpenIdConstants.StandardScopes.ScimScope,
            OpenBankingApiConstants.OpenBankingApiScopes.Accounts,
        };

        public static List<AuthenticationContextClassReference> AcrLst => new List<AuthenticationContextClassReference>
        {
            new AuthenticationContextClassReference
            {
                DisplayName = "Authenticate without using SCA",
                Name = "urn:openbanking:psd2:ca",
                AuthenticationMethodReferences = new List<string>
                {
                    "pwd"
                }
            }
        };

        public static ConcurrentBag<AccountAggregate> Accounts = new ConcurrentBag<AccountAggregate>
        {
            new AccountAggregate
            {
                AggregateId = "22289",
                Subject = "sub",
                Status = AccountStatus.Enabled,
                StatusUpdateDateTime = DateTime.UtcNow,
                AccountSubType = AccountSubTypes.CurrentAccount,
                Accounts = new List<CashAccount>
                {
                    new CashAccount
                    {
                        Identification = "80200110203345",
                        SecondaryIdentification = "00021"
                    }
                }
            },
            new AccountAggregate
            {
                AggregateId = "31820",
                Subject = "sub",
                Status = AccountStatus.Enabled,
                StatusUpdateDateTime = DateTime.UtcNow,
                AccountSubType = AccountSubTypes.Savings,
                Accounts = new List<CashAccount>
                {
                    new CashAccount
                    {
                        Identification = "10-159-60",
                        SecondaryIdentification = "789012345"
                    }
                }
            }
        };

        public static List<OAuthUser> Users => new List<OAuthUser>
        {
            new OAuthUser
            {
                Id = "sub",
                Credentials = new List<OAuthUserCredential>
                {
                    new OAuthUserCredential
                    {
                        CredentialType = "pwd",
                        Value = PasswordHelper.ComputeHash("password")
                    }
                },
                CreateDateTime = DateTime.Now,
                UpdateDateTime = DateTime.Now,
                DeviceRegistrationToken = "ciyortoPQHGluxo-vIZLu7:APA91bHRrB-mdgHl6IQFu4XNWR5VBXxOjaq-gAAuxCzswQAGeryvFaBqoJqJN_oSEtPZMTknRe2rixJj5cjnaWkCin8NSXm7Gug6peZd9EpJgJ98CNHqOudcFv_h3jp4dpgWn6imb7sR",
                Claims = new List<Claim>
                {
                    new Claim(Jwt.Constants.UserClaims.Subject, "sub"),
                    new Claim(Jwt.Constants.UserClaims.Name, "name"),
                    new Claim(Jwt.Constants.UserClaims.FamilyName, "familyName"),
                    new Claim(Jwt.Constants.UserClaims.UniqueName, "uniquename"),
                    new Claim(Jwt.Constants.UserClaims.GivenName, "givenName"),
                    new Claim(Jwt.Constants.UserClaims.MiddleName, "middleName"),
                    new Claim(Jwt.Constants.UserClaims.NickName, "nickName"),
                    new Claim(Jwt.Constants.UserClaims.BirthDate, "07-10-1989"),
                    new Claim(Jwt.Constants.UserClaims.PreferredUserName, "preferredUserName"),
                    new Claim(Jwt.Constants.UserClaims.ZoneInfo, "zoneInfo"),
                    new Claim(Jwt.Constants.UserClaims.Locale, "locale"),
                    new Claim(Jwt.Constants.UserClaims.Picture, "picture"),
                    new Claim(Jwt.Constants.UserClaims.WebSite, "website"),
                    new Claim(Jwt.Constants.UserClaims.Profile, "profile"),
                    new Claim(Jwt.Constants.UserClaims.Gender, "gender"),
                    new Claim(Jwt.Constants.UserClaims.Email, "agentsimpleidserver@gmail.com"),
                    new Claim(Jwt.Constants.UserClaims.UpdatedAt, "1612355959", Jwt.ClaimValueTypes.INTEGER),
                    new Claim(Jwt.Constants.UserClaims.EmailVerified, "true", Jwt.ClaimValueTypes.BOOLEAN),
                    new Claim(Jwt.Constants.UserClaims.Address, "{ 'street_address': '1234 Hollywood Blvd.', 'locality': 'Los Angeles', 'region': 'CA', 'postal_code': '90210', 'country': 'US' }", Jwt.ClaimValueTypes.JSONOBJECT),
                    new Claim(Jwt.Constants.UserClaims.PhoneNumber, "+1 (310) 123-4567"),
                    new Claim(Jwt.Constants.UserClaims.PhoneNumberVerified, "true", Jwt.ClaimValueTypes.BOOLEAN)
                }
            }
        };

        public static List<OpenIdClient> GetClients(JsonWebKey firstMtlsClientJsonWebKey, JsonWebKey secondMtlsClientJsonWebKey)
        {
            return new List<OpenIdClient>
            {
                new OpenIdClient
                {
                    ClientId = "firstMtlsClient",
                    ClientSecret = "mtsClientSecret",
                    ApplicationType = "web",
                    UpdateDateTime = DateTime.UtcNow,
                    CreateDateTime = DateTime.UtcNow,
                    TokenExpirationTimeInSeconds = 60 * 30,
                    RefreshTokenExpirationTimeInSeconds = 60 * 30,
                    TokenSignedResponseAlg = "PS256",
                    IdTokenSignedResponseAlg = "PS256",
                    RequestObjectSigningAlg = "PS256",
                    AllowedScopes = new List<OAuthScope>
                    {
                        SIDOpenIdConstants.StandardScopes.Email,
                        SIDOpenIdConstants.StandardScopes.OpenIdScope,
                        SIDOpenIdConstants.StandardScopes.Phone,
                        SIDOpenIdConstants.StandardScopes.Profile,
                        SIDOpenIdConstants.StandardScopes.Role,
                        OpenBankingApiConstants.OpenBankingApiScopes.Accounts
                    },
                    RedirectionUrls = new List<string>
                    {
                        "http://localhost:8080/test/a/simpleidserverOpenBankingApi/callback",
                        "https://www.certification.openid.net/test/a/simpleidserverOpenBankingApi/callback"
                    },
                    TokenEndPointAuthMethod = "tls_client_auth",
                    ResponseTypes = new List<string>
                    {
                        "id_token", "token", "code"
                    },
                    GrantTypes = new List<string>
                    {
                        "authorization_code", "implicit", "client_credentials", "refresh_token"
                    },
                    JsonWebKeys = new List<JsonWebKey>
                    {
                        firstMtlsClientJsonWebKey
                    },
                    TlsClientAuthSubjectDN = "CN=firstMtlsClient",
                    PreferredTokenProfile = "Bearer"
                },
                new OpenIdClient
                {
                    ClientId = "secondMtlsClient",
                    ClientSecret = "mtsClientSecret",
                    ApplicationType = "web",
                    UpdateDateTime = DateTime.UtcNow,
                    CreateDateTime = DateTime.UtcNow,
                    TokenExpirationTimeInSeconds = 60 * 30,
                    RefreshTokenExpirationTimeInSeconds = 60 * 30,
                    TokenSignedResponseAlg = "PS256",
                    IdTokenSignedResponseAlg = "PS256",
                    RequestObjectSigningAlg = "PS256",
                    AllowedScopes = new List<OAuthScope>
                    {
                        SIDOpenIdConstants.StandardScopes.Email,
                        SIDOpenIdConstants.StandardScopes.OpenIdScope,
                        SIDOpenIdConstants.StandardScopes.Phone,
                        SIDOpenIdConstants.StandardScopes.Profile,
                        SIDOpenIdConstants.StandardScopes.Role,
                        OpenBankingApiConstants.OpenBankingApiScopes.Accounts
                    },
                    TokenEndPointAuthMethod = "tls_client_auth",
                    ResponseTypes = new List<string>
                    {
                        "id_token", "token", "code"
                    },
                    GrantTypes = new List<string>
                    {
                        "authorization_code", "implicit", "client_credentials", "refresh_token"
                    },
                    RedirectionUrls = new List<string>
                    {
                        "http://localhost:8080/test/a/simpleidserverOpenBankingApi/callback?dummy1=lorem&dummy2=ipsum",
                        "https://www.certification.openid.net/test/a/simpleidserverOpenBankingApi/callback?dummy1=lorem&dummy2=ipsum"
                    },
                    JsonWebKeys = new List<JsonWebKey>
                    {
                        secondMtlsClientJsonWebKey
                    },
                    TlsClientAuthSubjectDN = "CN=secondMtlsClient",
                    PreferredTokenProfile = "Bearer"
                },
                new OpenIdClient
                {
                    ClientId = "firstPrivateKeyJwtClient",
                    ClientSecret = "privateKeyJwtClientSecret",
                    ApplicationType = "web",
                    UpdateDateTime = DateTime.UtcNow,
                    CreateDateTime = DateTime.UtcNow,
                    TokenExpirationTimeInSeconds = 60 * 30,
                    RefreshTokenExpirationTimeInSeconds = 60 * 30,
                    TokenSignedResponseAlg = "PS256",
                    IdTokenSignedResponseAlg = "PS256",
                    RequestObjectSigningAlg = "PS256",
                    AllowedScopes = new List<OAuthScope>
                    {
                        SIDOpenIdConstants.StandardScopes.Email,
                        SIDOpenIdConstants.StandardScopes.OpenIdScope,
                        SIDOpenIdConstants.StandardScopes.Phone,
                        SIDOpenIdConstants.StandardScopes.Profile,
                        SIDOpenIdConstants.StandardScopes.Role,
                        OpenBankingApiConstants.OpenBankingApiScopes.Accounts
                    },
                    RedirectionUrls = new List<string>
                    {
                        "http://localhost:8080/test/a/simpleidserverOpenBankingApi/callback",
                        "https://www.certification.openid.net/test/a/simpleidserverOpenBankingApi/callback"
                    },
                    TokenEndPointAuthMethod = "private_key_jwt",
                    ResponseTypes = new List<string>
                    {
                        "id_token", "token", "code"
                    },
                    GrantTypes = new List<string>
                    {
                        "authorization_code", "implicit", "client_credentials", "refresh_token"
                    },
                    JsonWebKeys = new List<JsonWebKey>
                    {
                        firstMtlsClientJsonWebKey
                    },
                    TlsClientAuthSubjectDN = "firstMtlsClient",
                    PreferredTokenProfile = "Bearer"
                },
                new OpenIdClient
                {
                    ClientId = "secondPrivateKeyJwtClient",
                    ClientSecret = "privateKeyClientSecret",
                    ApplicationType = "web",
                    UpdateDateTime = DateTime.UtcNow,
                    CreateDateTime = DateTime.UtcNow,
                    TokenExpirationTimeInSeconds = 60 * 30,
                    RefreshTokenExpirationTimeInSeconds = 60 * 30,
                    TokenSignedResponseAlg = "PS256",
                    IdTokenSignedResponseAlg = "PS256",
                    RequestObjectSigningAlg = "PS256",
                    AllowedScopes = new List<OAuthScope>
                    {
                        SIDOpenIdConstants.StandardScopes.Email,
                        SIDOpenIdConstants.StandardScopes.OpenIdScope,
                        SIDOpenIdConstants.StandardScopes.Phone,
                        SIDOpenIdConstants.StandardScopes.Profile,
                        SIDOpenIdConstants.StandardScopes.Role,
                        OpenBankingApiConstants.OpenBankingApiScopes.Accounts
                    },
                    TokenEndPointAuthMethod = "private_key_jwt",
                    ResponseTypes = new List<string>
                    {
                        "id_token", "token", "code"
                    },
                    GrantTypes = new List<string>
                    {
                        "authorization_code", "implicit", "client_credentials", "refresh_token"
                    },
                    RedirectionUrls = new List<string>
                    {
                        "http://localhost:8080/test/a/simpleidserverOpenBankingApi/callback?dummy1=lorem&dummy2=ipsum",
                        "https://www.certification.openid.net/test/a/simpleidserverOpenBankingApi/callback?dummy1=lorem&dummy2=ipsum"
                    },
                    JsonWebKeys = new List<JsonWebKey>
                    {
                        secondMtlsClientJsonWebKey
                    },
                    TlsClientAuthSubjectDN = "secondMtlsClient",
                    PreferredTokenProfile = "Bearer"
                },
                new OpenIdClient
                {
                    ClientId = "native",
                    ClientSecret = "nativeSecret",
                    TokenEndPointAuthMethod = "pkce",
                    ApplicationType = "web",
                    UpdateDateTime = DateTime.UtcNow,
                    CreateDateTime = DateTime.UtcNow,
                    TokenExpirationTimeInSeconds = 60 * 30,
                    RefreshTokenExpirationTimeInSeconds = 60 * 30,
                    TokenSignedResponseAlg = "PS256",
                    IdTokenSignedResponseAlg = "PS256",
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
                        "code",
                        "token"
                    }
                }
            };
        }
    }
}