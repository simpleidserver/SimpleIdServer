// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Domains;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace SimpleIdServer.IdServer
{
    public static class Constants
    {
        public static class JWKUsages
        {
            public const string Enc = "enc";
            public const string Sig = "sig";
        }

        public static List<string> RealmStandardUsers = new List<string>
        {
            "administrator"
        };

        public static List<string> RealmStandardClients = new List<string>
        {
            "website",
            "urn:website"
        };

        public static List<string> RealmStandardScopes = new List<string>
        {
            StandardScopes.OpenIdScope.Name,
            StandardScopes.Profile.Name,
            StandardScopes.SAMLProfile.Name
        };

        public static class StandardAuthorizationDetails
        {
            public const string OpenIdCredential = "openid_credential";
            public const string CredentialConfigurationId = "credential_configuration_id";
        }

        public static class EndPoints
        {
            public const string Token = "token";
            public const string TokenRevoke = "token/revoke";
            public const string TokenInfo = "token_info";
            public const string Jwks = "jwks";
            public const string Authorization = "authorization";
            public const string Registration = "register";
            public const string OAuthConfiguration = ".well-known/oauth-authorization-server";
            public const string OpenIDConfiguration = ".well-known/openid-configuration";
            public const string IdServerConfiguration = ".well-known/idserver-configuration";
            public const string FidoConfiguration = ".well-known/fido-configuration";
            public const string UMAConfiguration = ".well-known/uma2-configuration";
            public const string Form = "form";
            public const string AuthSchemeProviders = "authschemeproviders";
            public const string ClientManagement = "management/clients";
            public const string BCAuthorize = "bc-authorize";
            public const string BCCallback = "bc-callback";
            public const string MtlsPrefix = "mtls";
            public const string MtlsToken = MtlsPrefix + "/" + Token;
            public const string MtlsBCAuthorize = MtlsPrefix + "/" + BCAuthorize;
            public const string UserInfo = "userinfo";
            public const string CheckSession = "check_session";
            public const string EndSession = "end_session";
            public const string EndSessionCallback = "end_session_callback";
            public const string ActiveSession = "active_session";
            public const string Grants = "grants";
            public const string UMAPermissions = "perm";
            public const string UMAResources = "rreguri";
            public const string IdentityProvisioning = "provisioning";
            public const string PushedAuthorizationRequest = "par";
            public const string Users = "users";
            public const string Networks = "networks";
            public const string DeviceAuthorization = "device_authorization";
            public const string AuthenticationClassReferences = "acrs";
            public const string AuthenticationSchemeProviders = "idproviders";
            public const string FIDORegistration = "fido/u2f/registration";
            public const string FIDOAuthentication = "fido/u2f/authentication";
            public const string AuthMethods = "authmethods";
            public const string RegistrationWorkflows = "registrationworkflows";
            public const string ApiResources = "apiresources";
            public const string Scopes = "scopes";
            public const string Auditing = "auditing";
            public const string CertificateAuthorities = "cas";
            public const string Clients = "clients";
            public const string Statistics = "stats";
            public const string Realms = "realms";
            public const string Groups = "groups";
            public const string Languages = "languages";
        }

        public static List<string> AllStandardNotificationModes = new List<string>
        {
            StandardNotificationModes.Ping,
            StandardNotificationModes.Poll,
            StandardNotificationModes.Push
        };

        public static List<string> AllStandardGrantManagementActions = new List<string>
        {
            StandardGrantManagementActions.Create,
            StandardGrantManagementActions.Merge,
            StandardGrantManagementActions.Replace
        };

        public static class StandardGrantManagementActions
        {
            public const string Create = "create";
            public const string Merge = "merge";
            public const string Replace = "replace";
        }

        public static class StandardNotificationModes
        {
            public const string Poll = "poll";
            public const string Ping = "ping";
            public const string Push = "push";
        }

        public static class ScopeNames
        {
            public const string Register = "register";
            public const string AuthSchemeProvider = "manage_authschemeprovider";
        }

        public static class CertificateOIDS
        {
            public const string SubjectAlternativeName = "2.5.29.17";
        }

        public class Policies
        {
            public const string Authenticated = "authenticated";
        }

        public static ICollection<string> AllSigningAlgs = new List<string>
        {
            // SecurityAlgorithms.HmacSha256,
            // SecurityAlgorithms.HmacSha384,
            // SecurityAlgorithms.HmacSha512,
            // SecurityAlgorithms.RsaSsaPssSha256,
            // SecurityAlgorithms.RsaSsaPssSha384,
            // SecurityAlgorithms.RsaSsaPssSha512,
            SecurityAlgorithms.RsaSha256,
            SecurityAlgorithms.RsaSha384,
            SecurityAlgorithms.RsaSha512,
            SecurityAlgorithms.EcdsaSha256,
            SecurityAlgorithms.EcdsaSha384,
            SecurityAlgorithms.EcdsaSha512,
            SecurityAlgorithms.None
        };

        public static ICollection<string> AllEncAlgs = new List<string>
        {
            SecurityAlgorithms.RsaPKCS1,
            SecurityAlgorithms.RsaOAEP,
            // SecurityAlgorithms.Aes128KW,
            // SecurityAlgorithms.Aes192KW,
            // SecurityAlgorithms.Aes256KW,
            // SecurityAlgorithms.EcdhEs,
            // SecurityAlgorithms.EcdhEsA128kw,
            // SecurityAlgorithms.EcdhEsA192kw,
            // SecurityAlgorithms.EcdhEsA256kw
        };

        public static ICollection<string> AllEncryptions = new List<string>
        {
            SecurityAlgorithms.Aes128CbcHmacSha256,
            SecurityAlgorithms.Aes192CbcHmacSha384,
            SecurityAlgorithms.Aes256CbcHmacSha512,
            // SecurityAlgorithms.Aes128Gcm,
            // SecurityAlgorithms.Aes192Gcm,
            // SecurityAlgorithms.Aes256Gcm
        };

        public static class UserClaims
        {
            public static string MiddleName = "middle_name";
            public static string NickName = "nickname";
            public static string PreferredUserName = "preferred_username";
            public static string Profile = "profile";
            public static string Picture = "picture";
            public static string BirthDate = "birthdate";
            public static string ZoneInfo = "zoneinfo";
            public static string Locale = "locale";
            public static string UpdatedAt = "updated_at";
            public static string EmailVerified = "email_verified";
            public static string Address = "address";
            public static string Role = "role";
            public static string ScimId = "scim_id";
            public static string ScimLocation = "scim_location";
            public static string Events = "events";
        }

        public static class StandardClaims
        {
            public static ScopeClaimMapper Subject = ScopeClaimMapper.CreateOpenIdSubjectClaim();
            public static ScopeClaimMapper Name = ScopeClaimMapper.CreateOpenIdPropertyClaim(JwtRegisteredClaimNames.Name, JwtRegisteredClaimNames.Name, nameof(User.Firstname));
            public static ScopeClaimMapper FamilyName = ScopeClaimMapper.CreateOpenIdPropertyClaim(JwtRegisteredClaimNames.FamilyName, JwtRegisteredClaimNames.FamilyName, nameof(User.Lastname));
            public static ScopeClaimMapper UpdatedAt = ScopeClaimMapper.CreateOpenIdPropertyClaim(UserClaims.UpdatedAt, UserClaims.UpdatedAt, nameof(User.UpdateDateTime), TokenClaimJsonTypes.DATETIME);
            public static ScopeClaimMapper Email = ScopeClaimMapper.CreateOpenIdPropertyClaim(JwtRegisteredClaimNames.Email, JwtRegisteredClaimNames.Email, nameof(User.Email));
            public static ScopeClaimMapper EmailVerified = ScopeClaimMapper.CreateOpenIdPropertyClaim(UserClaims.EmailVerified, UserClaims.EmailVerified, nameof(User.EmailVerified), TokenClaimJsonTypes.BOOLEAN);
            public static ScopeClaimMapper UniqueName = ScopeClaimMapper.CreateOpenIdAttributeClaim(JwtRegisteredClaimNames.UniqueName, JwtRegisteredClaimNames.UniqueName, JwtRegisteredClaimNames.UniqueName);
            public static ScopeClaimMapper GivenName = ScopeClaimMapper.CreateOpenIdAttributeClaim(JwtRegisteredClaimNames.GivenName, JwtRegisteredClaimNames.GivenName, JwtRegisteredClaimNames.GivenName); 
            public static ScopeClaimMapper MiddleName = ScopeClaimMapper.CreateOpenIdAttributeClaim(UserClaims.MiddleName, UserClaims.MiddleName, UserClaims.MiddleName); 
            public static ScopeClaimMapper NickName = ScopeClaimMapper.CreateOpenIdAttributeClaim(UserClaims.NickName, UserClaims.NickName, UserClaims.NickName); 
            public static ScopeClaimMapper PreferredUserName = ScopeClaimMapper.CreateOpenIdAttributeClaim(UserClaims.PreferredUserName, UserClaims.PreferredUserName, UserClaims.PreferredUserName); 
            public static ScopeClaimMapper Profile = ScopeClaimMapper.CreateOpenIdAttributeClaim(UserClaims.Profile, UserClaims.Profile, UserClaims.Profile); 
            public static ScopeClaimMapper Picture = ScopeClaimMapper.CreateOpenIdAttributeClaim(UserClaims.Picture, UserClaims.Picture, UserClaims.Picture); 
            public static ScopeClaimMapper WebSite = ScopeClaimMapper.CreateOpenIdAttributeClaim(JwtRegisteredClaimNames.Website, JwtRegisteredClaimNames.Website, JwtRegisteredClaimNames.Website); 
            public static ScopeClaimMapper Gender = ScopeClaimMapper.CreateOpenIdAttributeClaim(JwtRegisteredClaimNames.Gender, JwtRegisteredClaimNames.Gender, JwtRegisteredClaimNames.Gender);
            public static ScopeClaimMapper BirthDate = ScopeClaimMapper.CreateOpenIdAttributeClaim(JwtRegisteredClaimNames.Birthdate, JwtRegisteredClaimNames.Birthdate, JwtRegisteredClaimNames.Birthdate);
            public static ScopeClaimMapper ZoneInfo = ScopeClaimMapper.CreateOpenIdAttributeClaim(UserClaims.ZoneInfo, UserClaims.ZoneInfo, UserClaims.ZoneInfo); 
            public static ScopeClaimMapper Locale = ScopeClaimMapper.CreateOpenIdAttributeClaim(UserClaims.Locale, UserClaims.Locale, UserClaims.Locale);
            public static List<ScopeClaimMapper> Address = ScopeClaimMapper.CreateOpenIdAddressClaim();
            public static ScopeClaimMapper PhoneNumber = ScopeClaimMapper.CreateOpenIdAttributeClaim(JwtRegisteredClaimNames.PhoneNumber, JwtRegisteredClaimNames.PhoneNumber, JwtRegisteredClaimNames.PhoneNumber);
            public static ScopeClaimMapper PhoneNumberVerified = ScopeClaimMapper.CreateOpenIdAttributeClaim(JwtRegisteredClaimNames.PhoneNumberVerified, JwtRegisteredClaimNames.PhoneNumberVerified, JwtRegisteredClaimNames.PhoneNumberVerified, TokenClaimJsonTypes.BOOLEAN);
            public static ScopeClaimMapper Role = ScopeClaimMapper.CreateOpenIdAttributeClaimArray(UserClaims.Role, UserClaims.Role, UserClaims.Role);
            public static ScopeClaimMapper ScimId = ScopeClaimMapper.CreateOpenIdAttributeClaim("scim_id", "scim_id", "scim_id");
            public static ScopeClaimMapper SAMLNameIdentifier = ScopeClaimMapper.CreateSAMLNameIdentifierClaim();
            public static ScopeClaimMapper SAMLName = ScopeClaimMapper.CreateSAMLPropertyClaim("name", ClaimTypes.Name, nameof(User.Firstname));
        }

        public static ICollection<AuthenticationSchemeProviderMapper> GetNegotiateIdProviderMappers() => new List<AuthenticationSchemeProviderMapper>
        {
            new AuthenticationSchemeProviderMapper
            {
                Id = Guid.NewGuid().ToString(),
                MapperType = MappingRuleTypes.IDENTIFIER,
                Name = "Identifier",
                SourceClaimName = ClaimTypes.PrimarySid
            },
            new AuthenticationSchemeProviderMapper
            {
                Id = Guid.NewGuid().ToString(),
                MapperType = MappingRuleTypes.SUBJECT,
                Name = "Name",
                SourceClaimName = ClaimTypes.Name,
                TargetUserProperty = nameof(User.Name)
            }
        };

        public static ICollection<AuthenticationSchemeProviderMapper> GetDefaultIdProviderMappers() =>new List<AuthenticationSchemeProviderMapper>
        {
            new AuthenticationSchemeProviderMapper
            {
                Id = Guid.NewGuid().ToString(),
                MapperType = MappingRuleTypes.SUBJECT,
                Name = "Subject",
                SourceClaimName = ClaimTypes.NameIdentifier
            },
            new AuthenticationSchemeProviderMapper
            {
                Id = Guid.NewGuid().ToString(),
                MapperType = MappingRuleTypes.USERPROPERTY,
                Name = "Firstname",
                SourceClaimName = ClaimTypes.Name,
                TargetUserProperty = nameof(User.Firstname)
            },
            new AuthenticationSchemeProviderMapper
            {
                Id = Guid.NewGuid().ToString(),
                MapperType = MappingRuleTypes.USERPROPERTY,
                Name = "Lastname",
                SourceClaimName = ClaimTypes.GivenName,
                TargetUserProperty = nameof(User.Lastname)
            },
            new AuthenticationSchemeProviderMapper
            {
                Id = Guid.NewGuid().ToString(),
                MapperType = MappingRuleTypes.USERPROPERTY,
                Name = "Email",
                SourceClaimName = ClaimTypes.Email,
                TargetUserProperty = nameof(User.Email)
            },
            new AuthenticationSchemeProviderMapper
            {
                Id = Guid.NewGuid().ToString(),
                MapperType = MappingRuleTypes.USERATTRIBUTE,
                Name = "Gender",
                SourceClaimName = ClaimTypes.Gender,
                TargetUserAttribute = JwtRegisteredClaimNames.Gender
            },
            new AuthenticationSchemeProviderMapper
            {
                Id = Guid.NewGuid().ToString(),
                MapperType = MappingRuleTypes.USERATTRIBUTE,
                Name = "DateOfBirth",
                SourceClaimName = ClaimTypes.DateOfBirth,
                TargetUserAttribute = JwtRegisteredClaimNames.Birthdate
            }
        };

        public static class StandardScopes
        {
            public static Scope Profile = new Scope
            {
                Id = Guid.NewGuid().ToString(),
                Name = "profile",
                ClaimMappers = new List<ScopeClaimMapper>
                {
                    StandardClaims.Name,
                    StandardClaims.FamilyName,
                    StandardClaims.UniqueName,
                    StandardClaims.GivenName,
                    StandardClaims.MiddleName,
                    StandardClaims.NickName,
                    StandardClaims.PreferredUserName,
                    StandardClaims.Profile,
                    StandardClaims.Picture,
                    StandardClaims.WebSite,
                    StandardClaims.Gender,
                    StandardClaims.BirthDate,
                    StandardClaims.ZoneInfo,
                    StandardClaims.Locale,
                    StandardClaims.UpdatedAt
                },
                Realms = new List<Domains.Realm>
                {
                    StandardRealms.Master
                },
                Type = ScopeTypes.IDENTITY,
                Protocol = ScopeProtocols.OPENID,
                IsExposedInConfigurationEdp = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope Email = new Scope
            {
                Id = Guid.NewGuid().ToString(),
                Name = "email",
                ClaimMappers = new List<ScopeClaimMapper>
                {
                    StandardClaims.Email,
                    StandardClaims.EmailVerified
                },
                Realms = new List<Domains.Realm>
                {
                    StandardRealms.Master
                },
                Type = ScopeTypes.IDENTITY,
                Protocol = ScopeProtocols.OPENID,
                IsExposedInConfigurationEdp = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope Address = new Scope
            {
                Id = Guid.NewGuid().ToString(),
                Name = "address",
                ClaimMappers = StandardClaims.Address,
                Realms = new List<Domains.Realm>
                {
                    StandardRealms.Master
                },
                Type = ScopeTypes.IDENTITY,
                Protocol = ScopeProtocols.OPENID,
                IsExposedInConfigurationEdp = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope Phone = new Scope
            {
                Id = Guid.NewGuid().ToString(),
                Name = "phone",
                ClaimMappers = new List<ScopeClaimMapper>
                {
                    StandardClaims.PhoneNumber,
                    StandardClaims.PhoneNumberVerified
                },
                Realms = new List<Domains.Realm>
                {
                    StandardRealms.Master
                },
                Type = ScopeTypes.IDENTITY,
                Protocol = ScopeProtocols.OPENID,
                IsExposedInConfigurationEdp = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope Role = new Scope
            {
                Id = Guid.NewGuid().ToString(),
                Name = "role",
                ClaimMappers = new List<ScopeClaimMapper>
                {
                    StandardClaims.Role
                },
                Realms = new List<Domains.Realm>
                {
                    StandardRealms.Master
                },
                Type = ScopeTypes.IDENTITY,
                Protocol = ScopeProtocols.OPENID,
                IsExposedInConfigurationEdp = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope OpenIdScope = new Scope
            {
                Id = Guid.NewGuid().ToString(),
                Name = "openid",
                ClaimMappers = new List<ScopeClaimMapper>
                {
                    StandardClaims.Subject
                },
                Realms = new List<Domains.Realm>
                {
                    StandardRealms.Master
                },
                Type = ScopeTypes.IDENTITY,
                Protocol = ScopeProtocols.OPENID,
                IsExposedInConfigurationEdp = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope OfflineAccessScope = new Scope
            {
                Id = Guid.NewGuid().ToString(),
                Type = ScopeTypes.APIRESOURCE,
                Protocol = ScopeProtocols.OAUTH,
                Name = "offline_access",
                Realms = new List<Domains.Realm>
                {
                    StandardRealms.Master
                },
                IsExposedInConfigurationEdp = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope ScimScope = new Scope
            {
                Id = Guid.NewGuid().ToString(),
                Type = ScopeTypes.IDENTITY,
                Name = "scim",
                IsExposedInConfigurationEdp = true,
                ClaimMappers = new List<ScopeClaimMapper>
                {
                    StandardClaims.ScimId
                },
                Realms = new List<Domains.Realm>
                {
                    StandardRealms.Master
                },
                Protocol = ScopeProtocols.OPENID,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope GrantManagementQuery = new Scope
            {
                Id = Guid.NewGuid().ToString(),
                Type = ScopeTypes.IDENTITY,
                Name = "grant_management_query",
                Realms = new List<Domains.Realm>
                {
                    StandardRealms.Master
                },
                Protocol = ScopeProtocols.OPENID,
                IsExposedInConfigurationEdp = false,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope GrantManagementRevoke = new Scope
            {
                Id = Guid.NewGuid().ToString(),
                Type = ScopeTypes.IDENTITY,
                Name = "grant_management_revoke",
                Realms = new List<Domains.Realm>
                {
                    StandardRealms.Master
                },
                Protocol = ScopeProtocols.OPENID,
                IsExposedInConfigurationEdp = false,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope UmaProtection = new Scope
            {
                Id = Guid.NewGuid().ToString(),
                Type = ScopeTypes.IDENTITY,
                Name = "uma_protection",
                Realms = new List<Domains.Realm>
                {
                    StandardRealms.Master
                },
                Protocol = ScopeProtocols.OPENID,
                IsExposedInConfigurationEdp = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope Users = new Scope
            {
                Id = Guid.NewGuid().ToString(),
                Type = ScopeTypes.APIRESOURCE,
                Name = "users",
                Realms = new List<Domains.Realm>
                {
                    StandardRealms.Master
                },
                Protocol = ScopeProtocols.OAUTH,
                IsExposedInConfigurationEdp = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope AuthenticationSchemeProviders = new Scope
            {
                Id = Guid.NewGuid().ToString(),
                Type = ScopeTypes.APIRESOURCE,
                Name = "authenticationschemeproviders",
                Realms = new List<Domains.Realm>
                {
                    StandardRealms.Master
                },
                Protocol = ScopeProtocols.OAUTH,
                IsExposedInConfigurationEdp = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope AuthenticationMethods = new Scope
            {
                Id = Guid.NewGuid().ToString(),
                Type = ScopeTypes.APIRESOURCE,
                Name = "authenticationmethods",
                Realms = new List<Domains.Realm>
                {
                    StandardRealms.Master
                },
                Protocol = ScopeProtocols.OAUTH,
                IsExposedInConfigurationEdp = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope RegistrationWorkflows = new Scope
            {
                Id = Guid.NewGuid().ToString(),
                Type = ScopeTypes.APIRESOURCE,
                Name = "registrationworkflows",
                Realms = new List<Domains.Realm>
                {
                    StandardRealms.Master
                },
                Protocol = ScopeProtocols.OAUTH,
                IsExposedInConfigurationEdp = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope Register = new Scope
            {
                Id = Guid.NewGuid().ToString(),
                Type = ScopeTypes.APIRESOURCE,
                Name = "register",
                Realms = new List<Domains.Realm>
                {
                    StandardRealms.Master
                },
                Protocol = ScopeProtocols.OAUTH,
                IsExposedInConfigurationEdp = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope SAMLProfile = new Scope
            {
                Id = Guid.NewGuid().ToString(),
                Type = ScopeTypes.IDENTITY,
                Name = "saml_profile",
                Realms = new List<Domains.Realm>
                {
                    StandardRealms.Master
                },
                Protocol = ScopeProtocols.SAML,
                IsExposedInConfigurationEdp = false,
                ClaimMappers = new List<ScopeClaimMapper>
                {
                    StandardClaims.SAMLNameIdentifier,
                    StandardClaims.SAMLName
                },
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope Provisioning = new Scope
            {
                Id = Guid.NewGuid().ToString(),
                Type = ScopeTypes.APIRESOURCE,
                Name = "provisioning",
                Realms = new List<Domains.Realm>
                {
                    StandardRealms.Master
                },
                Protocol = ScopeProtocols.OAUTH,
                IsExposedInConfigurationEdp = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope CredentialConfigurations = new Scope
            {
                Id = Guid.NewGuid().ToString(),
                Type = ScopeTypes.APIRESOURCE,
                Name = "credconfs",
                Realms = new List<Domains.Realm>
                {
                    StandardRealms.Master
                },
                Protocol = ScopeProtocols.OAUTH,
                IsExposedInConfigurationEdp = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope CredentialInstances = new Scope
            {
                Id = Guid.NewGuid().ToString(),
                Type = ScopeTypes.APIRESOURCE,
                Name = "credinstances",
                Realms = new List<Domains.Realm>
                {
                    StandardRealms.Master
                },
                Protocol = ScopeProtocols.OAUTH,
                IsExposedInConfigurationEdp = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope Acrs = new Scope
            {
                Id = Guid.NewGuid().ToString(),
                Type = ScopeTypes.APIRESOURCE,
                Name = "acrs",
                Realms = new List<Domains.Realm>
                {
                    StandardRealms.Master
                },
                Protocol = ScopeProtocols.OAUTH,
                IsExposedInConfigurationEdp = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope ApiResources = new Scope
            {
                Id = Guid.NewGuid().ToString(),
                Type = ScopeTypes.APIRESOURCE,
                Name = "apiresources",
                Realms = new List<Domains.Realm>
                {
                    StandardRealms.Master
                },
                Protocol = ScopeProtocols.OAUTH,
                IsExposedInConfigurationEdp = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope Auditing = new Scope
            {
                Id = Guid.NewGuid().ToString(),
                Type = ScopeTypes.APIRESOURCE,
                Name = "auditing",
                Realms = new List<Domains.Realm>
                {
                    StandardRealms.Master
                },
                Protocol = ScopeProtocols.OAUTH,
                IsExposedInConfigurationEdp = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope Scopes = new Scope
            {
                Id = Guid.NewGuid().ToString(),
                Type = ScopeTypes.APIRESOURCE,
                Name = "scopes",
                Realms = new List<Domains.Realm>
                {
                    StandardRealms.Master
                },
                Protocol = ScopeProtocols.OAUTH,
                IsExposedInConfigurationEdp = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope CertificateAuthorities = new Scope
            {
                Id = Guid.NewGuid().ToString(),
                Type = ScopeTypes.APIRESOURCE,
                Name = "certificateauthorities",
                Realms = new List<Domains.Realm>
                {
                    StandardRealms.Master
                },
                Protocol = ScopeProtocols.OAUTH,
                IsExposedInConfigurationEdp = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope Clients = new Scope
            {
                Id = Guid.NewGuid().ToString(),
                Type = ScopeTypes.APIRESOURCE,
                Name = "clients",
                Realms = new List<Domains.Realm>
                {
                    StandardRealms.Master
                },
                Protocol = ScopeProtocols.OAUTH,
                IsExposedInConfigurationEdp = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope Realms = new Scope
            {
                Id = Guid.NewGuid().ToString(),
                Type = ScopeTypes.APIRESOURCE,
                Name = "realms",
                Realms = new List<Domains.Realm>
                {
                    StandardRealms.Master
                },
                Protocol = ScopeProtocols.OAUTH,
                IsExposedInConfigurationEdp = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope Groups = new Scope
            {
                Id = Guid.NewGuid().ToString(),
                Type = ScopeTypes.APIRESOURCE,
                Name = "groups",
                Realms = new List<Domains.Realm>
                {
                    StandardRealms.Master
                },
                Protocol = ScopeProtocols.OAUTH,
                IsExposedInConfigurationEdp = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
        }

        public static class StandardAcrs
        {
            public static AuthenticationContextClassReference FirstLevelAssurance = new AuthenticationContextClassReference
            {
                Id = Guid.NewGuid().ToString(),
                AuthenticationMethodReferences = new[] { Areas.Password },
                Name = "sid-load-01",
                DisplayName = "First level of assurance",
                UpdateDateTime = DateTime.UtcNow,
                Realms = new List<Domains.Realm>
                {
                    StandardRealms.Master
                }
            };
            public static AuthenticationContextClassReference IapSilver = new AuthenticationContextClassReference
            {
                Id = Guid.NewGuid().ToString(),
                AuthenticationMethodReferences = new[] { Areas.Password },
                Name = "urn:mace:incommon:iap:silver",
                DisplayName = "Silver",
                UpdateDateTime = DateTime.UtcNow,
                Realms = new List<Domains.Realm>
                {
                    StandardRealms.Master
                }
            };            
        }

        public static class StandardRealms
        {
            public static Domains.Realm Master = new Domains.Realm
            {
                Name = Constants.DefaultRealm,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
                Description = Constants.DefaultRealm
            };
        }

        public static ICollection<string> AllUserClaims = new List<string>
        {
            JwtRegisteredClaimNames.Sub, JwtRegisteredClaimNames.Name, JwtRegisteredClaimNames.GivenName, JwtRegisteredClaimNames.FamilyName, UserClaims.MiddleName,
            UserClaims.NickName, UserClaims.PreferredUserName, UserClaims.Profile, UserClaims.Picture, JwtRegisteredClaimNames.Website,
            JwtRegisteredClaimNames.Email, UserClaims.EmailVerified, JwtRegisteredClaimNames.Gender, UserClaims.BirthDate, UserClaims.ZoneInfo,
            UserClaims.Locale, JwtRegisteredClaimNames.PhoneNumber, JwtRegisteredClaimNames.PhoneNumberVerified, UserClaims.Address, UserClaims.UpdatedAt,
            UserClaims.Role, UserClaims.ScimId, UserClaims.ScimLocation
        };

        public static class Areas
        {
            public const string Password = "pwd";
        }

        public const string DefaultExternalCookieAuthenticationScheme = "ExternalCookies";
        public const string DefaultCertificateAuthenticationScheme = "Certificate";
        public static string AuthorizationHeaderName = "Authorization";
        public const string DPOPHeaderName = "DPoP";
        public const string DPOPNonceHeaderName = "DPoP-Nonce";
        /// <summary>
        /// Direct use of a shared symmetric key as the CEK.
        /// </summary>
        public const string AlgDir = "dir";
        public const string ParFormatKey = "urn:ietf:params:oauth:request_uri";
        public const string IdProviderSeparator = ";";
        public const string LDAPDistinguishedName = "LDAP_DISTINGUISHEDNAME";
        public const string DefaultNotificationMode = "console";
        public const string DefaultRealm = "master";
        public const string DefaultRealmCookieName = "CurrentRealm";
        public const string DefaultCurrentAmrCookieName = "currentAmr";
        public const string DefaultRememberMeCookieName = "RememberMe";
        public const string Prefix = "prefix";
        public const string DefaultLanguage = "en";
    }
}
