using SimpleIdServer.Common.Domains;
using SimpleIdServer.Common.Helpers;
using SimpleIdServer.OAuth.Domains;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.OpenID.Host.Acceptance.Tests
{
    class DefaultConfiguration
    {
        public static List<OAuthUser> Users => new List<OAuthUser>
        {
            new OAuthUser
            {
                Id = "administrator",
                Credentials = new List<UserCredential>
                {
                    new UserCredential
                    {
                        CredentialType = "pwd",
                        Value = PasswordHelper.ComputeHash("password")
                    }
                },
                Sessions =new List<UserSession>
                {
                    new UserSession
                    {
                        AuthenticationDateTime = DateTime.UtcNow,
                        ExpirationDateTime = DateTime.UtcNow.AddSeconds(5 * 60),
                        SessionId = Guid.NewGuid().ToString()
                    }
                },
                OAuthUserClaims = new List<UserClaim>
                {
                    new UserClaim(Jwt.Constants.UserClaims.Subject, "administrator"),
                    new UserClaim(Jwt.Constants.UserClaims.Name, "Thierry Habart"),
                    new UserClaim(Jwt.Constants.UserClaims.Email, "habarthierry@hotmail.fr"),
                    new UserClaim(Jwt.Constants.UserClaims.Role, "role1"),
                    new UserClaim(Jwt.Constants.UserClaims.Role, "role2"),
                    new UserClaim(Jwt.Constants.UserClaims.UpdatedAt, "1612361922", Jwt.ClaimValueTypes.INTEGER),
                    new UserClaim(Jwt.Constants.UserClaims.EmailVerified, "true", Jwt.ClaimValueTypes.BOOLEAN),
                    new UserClaim(Jwt.Constants.UserClaims.Address, "{ 'street_address': '1234 Hollywood Blvd.', 'region': 'CA' }", Jwt.ClaimValueTypes.JSONOBJECT)
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
