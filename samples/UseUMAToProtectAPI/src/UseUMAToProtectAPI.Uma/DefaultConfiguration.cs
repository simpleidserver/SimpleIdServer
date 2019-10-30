using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.Uma;
using System;
using System.Collections.Generic;

namespace UseUMAToProtectAPI.Uma
{
    public static class DefaultConfiguration
    {
        public static List<OAuthScope> DefaultScopes = new List<OAuthScope>
        {
            UMAConstants.StandardUMAScopes.UmaProtection
        };

        public static List<OAuthClient> DefaultClients = new List<OAuthClient>
        {
            new OAuthClient
            {
                ClientId = "api",
                Secrets = new List<ClientSecret>
                {
                    new ClientSecret(ClientSecretTypes.SharedSecret, PasswordHelper.ComputeHash("apiSecret"))
                },
                TokenEndPointAuthMethod = "client_secret_post",
                UpdateDateTime = DateTime.UtcNow,
                CreateDateTime = DateTime.UtcNow,
                TokenExpirationTimeInSeconds = 60 * 30,
                RefreshTokenExpirationTimeInSeconds = 60 * 30,
                TokenSignedResponseAlg = "RS256",
                AllowedScopes = new List<OAuthScope>
                {
                    UMAConstants.StandardUMAScopes.UmaProtection
                },
                GrantTypes = new List<string>
                {
                    "client_credentials"
                },
                RedirectionUrls = new List<string>
                {
                    "https://localhost:5001/signin-oidc"
                },
                PreferredTokenProfile = "Bearer",
                ResponseTypes = new List<string>
                {
                    "token"
                }
            },
            new OAuthClient
            {
                ClientId = "portal",
                Secrets = new List<ClientSecret>
                {
                    new ClientSecret(ClientSecretTypes.SharedSecret, PasswordHelper.ComputeHash("portalSecret"))
                },
                TokenEndPointAuthMethod = "client_secret_post",
                UpdateDateTime = DateTime.UtcNow,
                CreateDateTime = DateTime.UtcNow,
                TokenExpirationTimeInSeconds = 60 * 30,
                RefreshTokenExpirationTimeInSeconds = 60 * 30,
                TokenSignedResponseAlg = "RS256",
                AllowedScopes = new List<OAuthScope>
                {
                    UMAConstants.StandardUMAScopes.UmaProtection
                },
                GrantTypes = new List<string>
                {
                    "urn:ietf:params:oauth:grant-type:uma-ticket"
                },
                PreferredTokenProfile = "Bearer",
                ResponseTypes = new List<string>
                {
                    "token"
                }
            }
        };
    }
}
