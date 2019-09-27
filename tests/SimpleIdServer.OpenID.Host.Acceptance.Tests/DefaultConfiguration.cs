using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OpenID.Domains;
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
                    { SimpleIdServer.Jwt.Constants.UserClaims.Name, "Thierry Habart" },
                    { SimpleIdServer.Jwt.Constants.UserClaims.Email, "habarthierry@hotmail.fr" }
                }
            }
        };

        public static List<OpenIdScope> Scopes => new List<OpenIdScope>
        {
            new OpenIdScope
            {
                Name = "scope1",
                IsExposedInConfigurationEdp = true
            },
            new OpenIdScope
            {
                Name = "role",
                IsExposedInConfigurationEdp = true,
                Claims = new List<string>
                {
                    "role"
                }
            },
            new OpenIdScope
            {
                Name = "email",
                IsExposedInConfigurationEdp = true,
                Claims = new List<string>
                {
                    "email"
                }
            },
            new OpenIdScope
            {
                Name = "profile",
                IsExposedInConfigurationEdp = true,
                Claims = new List<string>
                {
                    "name"
                }
            },
            new OpenIdScope
            {
                Name = "offline_access",
                IsExposedInConfigurationEdp = true
            }
        };
    }
}
