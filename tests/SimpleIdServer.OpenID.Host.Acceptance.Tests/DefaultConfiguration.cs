using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OpenID.Domains;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace SimpleIdServer.OpenID.Host.Acceptance.Tests
{
    class DefaultConfiguration
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
                Sessions =new List<OAuthUserSession>
                {
                    new OAuthUserSession
                    {
                        AuthenticationDateTime = DateTime.UtcNow,
                        ExpirationDateTime = DateTime.UtcNow.AddSeconds(5 * 60),
                        SessionId = Guid.NewGuid().ToString()
                    }
                },
                OAuthUserClaims = new List<OAuthUserClaim>
                {
                    new OAuthUserClaim(Jwt.Constants.UserClaims.Subject, "administrator"),
                    new OAuthUserClaim(Jwt.Constants.UserClaims.Name, "Thierry Habart"),
                    new OAuthUserClaim(Jwt.Constants.UserClaims.Email, "habarthierry@hotmail.fr"),
                    new OAuthUserClaim(Jwt.Constants.UserClaims.Role, "role1"),
                    new OAuthUserClaim(Jwt.Constants.UserClaims.Role, "role2"),
                    new OAuthUserClaim(Jwt.Constants.UserClaims.UpdatedAt, "1612361922", Jwt.ClaimValueTypes.INTEGER),
                    new OAuthUserClaim(Jwt.Constants.UserClaims.EmailVerified, "true", Jwt.ClaimValueTypes.BOOLEAN),
                    new OAuthUserClaim(Jwt.Constants.UserClaims.Address, "{ 'street_address': '1234 Hollywood Blvd.', 'region': 'CA' }", Jwt.ClaimValueTypes.JSONOBJECT)
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
                IsExposedInConfigurationEdp = true,
                Claims = new List<OAuthScopeClaim>
                {
                    new OAuthScopeClaim("role", true)
                }
            },
            new OAuthScope
            {
                Name = "email",
                IsExposedInConfigurationEdp = true,
                Claims = new List<OAuthScopeClaim>
                {
                    new OAuthScopeClaim(Jwt.Constants.UserClaims.Email, true),
                    new OAuthScopeClaim(Jwt.Constants.UserClaims.EmailVerified, true)
                }
            },
            new OAuthScope
            {
                Name = "profile",
                IsExposedInConfigurationEdp = true,
                Claims = new List<OAuthScopeClaim>
                {
                    new OAuthScopeClaim(Jwt.Constants.UserClaims.Name, true),
                    new OAuthScopeClaim(Jwt.Constants.UserClaims.UpdatedAt, true)
                }
            },
            new OAuthScope
            {
                Name = "address",
                IsExposedInConfigurationEdp = true,
                Claims = new List<OAuthScopeClaim>
                {
                    new OAuthScopeClaim(Jwt.Constants.UserClaims.Address, true)
                }
            },
            new OAuthScope
            {
                Name = "offline_access",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "openid",
                Claims = new List<OAuthScopeClaim>
                {
                    new OAuthScopeClaim(Jwt.Constants.UserClaims.Subject, true)
                },
                IsExposedInConfigurationEdp = true
            }
        };
    }
}
