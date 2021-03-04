// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OpenID;
using SimpleIdServer.OpenID.Domains;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace $rootnamespace$
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
                Claims = new List<Claim>
                {
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.Subject, "sub"),
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.Name, "name"),
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.FamilyName, "familyName"),
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.UniqueName, "uniquename"),
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.GivenName, "givenName"),
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.MiddleName, "middleName"),
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.NickName, "nickName"),
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.BirthDate, "07-10-1989"),
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.PreferredUserName, "preferredUserName"),
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.ZoneInfo, "zoneInfo"),
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.Locale, "locale"),
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.Picture, "picture"),
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.WebSite, "website"),
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.Profile, "profile"),
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.Gender, "gender"),
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.Email, "agentsimpleidserver@gmail.com"),
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.UpdatedAt, "1612355959", SimpleIdServer.Jwt.ClaimValueTypes.INTEGER),
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.EmailVerified, "true", SimpleIdServer.Jwt.ClaimValueTypes.BOOLEAN),
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.Address, "{ 'street_address': '1234 Hollywood Blvd.', 'locality': 'Los Angeles', 'region': 'CA', 'postal_code': '90210', 'country': 'US' }", SimpleIdServer.Jwt.ClaimValueTypes.JSONOBJECT),
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.PhoneNumber, "+1 (310) 123-4567"),
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.PhoneNumberVerified, "true", SimpleIdServer.Jwt.ClaimValueTypes.BOOLEAN)
                }
            },
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
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.GivenName, "administrator"),
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.Role, "admin")
                }
            },
            new OAuthUser
            {
                Id = "businessanalyst",
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
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.Subject, "businessanalyst"),
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.GivenName, "businessanalyst"),
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.Role, "businessanalyst")
                }
            },
            new OAuthUser
            {
                Id = "caseworker",
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
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.Subject, "caseworker"),
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.GivenName, "caseworker"),
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.Role, "caseworker")
                }
            },
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
                Claims = new List<Claim>
                {
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.Subject, "scimUser"),
                    new Claim("scim_id", "1")
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
                Claims = new List<Claim>
                {
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.Subject, "umaUser"),
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.Name, "User"),
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.UniqueName, "User")
                }
            },
            new OAuthUser
            {
                Id = "doctor",
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
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.Subject, "doctor"),
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.Name, "Doctor"),
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.GivenName, "Doctor"),
                    new Claim(SimpleIdServer.Jwt.Constants.UserClaims.UniqueName, "Doctor")
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
                    Secrets = new List<ClientSecret>
                    {
                        new ClientSecret(ClientSecretTypes.SharedSecret, "scimClientSecret", null)
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
                            Claims = new List<OpenIdScopeClaim>
                            {
                                new OpenIdScopeClaim("scim_id", true)
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
                        new ClientSecret(ClientSecretTypes.SharedSecret, "umaClientSecret", null)
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
                    Secrets = new List<ClientSecret>
                    {
                        new ClientSecret(ClientSecretTypes.SharedSecret, "simpleIdServerWebsiteSecret", null)
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
                        SIDOpenIdConstants.StandardScopes.Profile,
                        SIDOpenIdConstants.StandardScopes.Email,
                        SIDOpenIdConstants.StandardScopes.Role
                    },
                    GrantTypes = new List<string>
                    {
                        "implicit"
                    },
                    RedirectionUrls = new List<string>
                    {
                        "http://localhost:4200",
                        "http://simpleidserver.northeurope.cloudapp.azure.com/simpleidserver"
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
                    ClientId = "tradWebsite",
                    Secrets = new List<ClientSecret>
                    {
                        new ClientSecret(ClientSecretTypes.SharedSecret, "tradWebsiteSecret", null)
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
                    ClientId = "native",
                    Secrets = new List<ClientSecret>
                    {
                        new ClientSecret(ClientSecretTypes.SharedSecret, "nativeSecret", null)
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
                },
                new OpenIdClient
                {
                    ClientId = "caseManagementWebsite",
                    Secrets = new List<ClientSecret>
                    {
                        new ClientSecret(ClientSecretTypes.SharedSecret, "b98113b5-f45f-4a4a-9db5-610b7183e148", null)
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
                        SIDOpenIdConstants.StandardScopes.Email,
                        SIDOpenIdConstants.StandardScopes.Role
                    },
                    GrantTypes = new List<string>
                    {
                        "implicit"
                    },
                    RedirectionUrls = new List<string>
                    {
                        "http://localhost:51724",
                        "http://localhost:8080",
                        "http://simpleidserver.northeurope.cloudapp.azure.com/casemanagement"
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
                    ClientId = "caseManagementTasklistWebsite",
                    Secrets = new List<ClientSecret>
                    {
                        new ClientSecret(ClientSecretTypes.SharedSecret, "b98113b5-f45f-4a4a-9db5-610b7183e148", null)
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
                        SIDOpenIdConstants.StandardScopes.Email,
                        SIDOpenIdConstants.StandardScopes.Role
                    },
                    GrantTypes = new List<string>
                    {
                        "implicit"
                    },
                    RedirectionUrls = new List<string>
                    {
                        "http://localhost:51724",
                        "http://localhost:8081",
                        "http://simpleidserver.northeurope.cloudapp.azure.com/tasklist"
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
                    ClientId = "caseManagementPerformanceWebsite",
                    Secrets = new List<ClientSecret>
                    {
                        new ClientSecret(ClientSecretTypes.SharedSecret, "91894b86-c57e-489a-838d-fb82621a67ee", null)
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
                        SIDOpenIdConstants.StandardScopes.Email,
                        SIDOpenIdConstants.StandardScopes.Role
                    },
                    GrantTypes = new List<string>
                    {
                        "implicit"
                    },
                    RedirectionUrls = new List<string>
                    {
                        "http://localhost:51725",
                        "http://localhost:8081",
                        "http://simpleidserver.northeurope.cloudapp.azure.com/casemanagementperformance"
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
                    ClientId = "medikitWebsite",
                    Secrets = new List<ClientSecret>
                    {
                        new ClientSecret(ClientSecretTypes.SharedSecret, "f200eeb0-a6a3-465e-be91-97806e5dd3bc", null)
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
                        SIDOpenIdConstants.StandardScopes.Email,
                        SIDOpenIdConstants.StandardScopes.Role
                    },
                    GrantTypes = new List<string>
                    {
                        "implicit"
                    },
                    RedirectionUrls = new List<string>
                    {
                        "http://localhost:8080",
                        "http://simpleidserver.northeurope.cloudapp.azure.com/medikit"
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