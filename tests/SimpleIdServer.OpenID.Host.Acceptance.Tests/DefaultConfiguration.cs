using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Domains.Scopes;
using SimpleIdServer.OAuth.Domains.Users;
using SimpleIdServer.OAuth.Helpers;
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

        public static List<OAuthScope> Scopes => new List<OAuthScope>
        {
            new OAuthScope
            {
                Name = "scope1",
                Descriptions = new List<OAuthTranslation>
                {
                    new OAuthTranslation("scope1_description", "Access to the scope1", "en"),
                    new OAuthTranslation("scope1_description", "Accéder au scope1", "fr")
                },
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "role",
                Descriptions = new List<OAuthTranslation>
                {
                    new OAuthTranslation("role_description", "Access to the role", "en"),
                    new OAuthTranslation("role_description", "Accéder au rôle", "fr")
                },
                IsExposedInConfigurationEdp = true,
                Claims = new List<string>
                {
                    "role"
                }
            },
            new OAuthScope
            {
                Name = "email",
                Descriptions = new List<OAuthTranslation>
                {
                    new OAuthTranslation("email_description", "Access to the email", "en"),
                    new OAuthTranslation("email_description", "Accéder à l'email", "fr")
                },
                IsExposedInConfigurationEdp = true,
                Claims = new List<string>
                {
                    "email"
                }
            },
            new OAuthScope
            {
                Name = "profile",
                Descriptions = new List<OAuthTranslation>
                {
                    new OAuthTranslation("profile_description", "Access to the profile", "en"),
                    new OAuthTranslation("profile_description", "Accéder au profil", "fr")
                },
                IsExposedInConfigurationEdp = true,
                Claims = new List<string>
                {
                    "name"
                }
            },
            new OAuthScope
            {
                Name = "offline_access",
                Descriptions = new List<OAuthTranslation>
                {
                    new OAuthTranslation("offline_access_description", "Offline access", "en"),
                    new OAuthTranslation("offline_access_description", "Accéder en hors ligne", "fr")
                },
                IsExposedInConfigurationEdp = true
            }
        };
    }
}
