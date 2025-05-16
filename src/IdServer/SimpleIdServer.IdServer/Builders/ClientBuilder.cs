// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Api.Authorization.ResponseTypes;
using SimpleIdServer.IdServer.Api.Token.Handlers;
using SimpleIdServer.IdServer.Authenticate.Handlers;
using SimpleIdServer.IdServer.Config;
using SimpleIdServer.IdServer.Domains;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Builders
{
    public class ClientBuilder
    {
        /// <summary>
        /// Build credential issuer client.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="realm"></param>
        /// <param name="redirectUrls"></param>
        /// <returns></returns>
        public static CredentialIssuerClientBuilder BuildCredentialIssuer(string clientId, string clientSecret, Domains.Realm realm = null, params string[] redirectUrls)
        {
            var client = new Client
            {
                Id = Guid.NewGuid().ToString(),
                ClientId = clientId,
                ClientType = ClientTypes.CREDENTIALISSUER,
                RedirectionUrls = redirectUrls,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
                ResponseTypes = new List<string> { AuthorizationCodeResponseTypeHandler.RESPONSE_TYPE },
                Secrets = new List<ClientSecret>
                {
                    ClientSecret.Create(clientSecret, HashAlgs.PLAINTEXT)
                }
            };
            if (realm == null) client.Realms.Add(Config.DefaultRealms.Master);
            else client.Realms.Add(realm);
            client.GrantTypes.Add(TokenExchangePreAuthorizedCodeHandler.GRANT_TYPE);
            client.GrantTypes.Add(AuthorizationCodeHandler.GRANT_TYPE);
            client.TokenEndPointAuthMethod = OAuthClientSecretPostAuthenticationHandler.AUTH_METHOD;
            return new CredentialIssuerClientBuilder(client);
        }

        /// <summary>
        /// Build wallet client - use credential offer - cross device (with information pre-submitted by the End-User).
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="realm"></param>
        /// <returns></returns>
        public static WalletClientBuilder BuildWalletClient(string clientId, string clientSecret, Domains.Realm realm = null)
        {
            var client = new Client
            {
                Id = Guid.NewGuid().ToString(),
                ClientId = clientId,
                ClientType = ClientTypes.WALLET,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
                Secrets = new List<ClientSecret>
                {
                    ClientSecret.Create(clientSecret, HashAlgs.PLAINTEXT)
                }
            };
            if (realm == null) client.Realms.Add(Config.DefaultRealms.Master);
            else client.Realms.Add(realm);
            client.AuthorizationDataTypes.Add("openid_credential");
            client.GrantTypes.Add(PreAuthorizedCodeHandler.GRANT_TYPE);
            client.GrantTypes.Add(AuthorizationCodeHandler.GRANT_TYPE);
            return new WalletClientBuilder(client);
        }

        /// <summary>
        /// Build client for REST.API.
        /// By default client_credentials grant-type is used to obtain an access token outside of the context of a user.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <returns></returns>
        public static ApiClientBuilder BuildApiClient(string clientId, string clientSecret, Domains.Realm realm = null)
        {
            var client = new Client
            {
                Id = Guid.NewGuid().ToString(),
                ClientId = clientId,
                ClientType = ClientTypes.MACHINE,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
                Secrets = new List<ClientSecret>
                {
                    ClientSecret.Create(clientSecret, HashAlgs.PLAINTEXT)
                }
            };
            if (realm == null) client.Realms.Add(Config.DefaultRealms.Master);
            else client.Realms.Add(realm);
            client.GrantTypes.Add(ClientCredentialsHandler.GRANT_TYPE);
            client.TokenEndPointAuthMethod = OAuthClientSecretPostAuthenticationHandler.AUTH_METHOD;
            return new ApiClientBuilder(client);
        }

        /// <summary>
        /// Build client for traditional website like ASP.NET CORE.
        /// By default authorization_code grant-type PKCE is used by confidential and public clients to exchange an authorization code for an access token.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="redirectUrls"></param>
        /// <returns></returns>
        public static TraditionalWebsiteClientBuilder BuildTraditionalWebsiteClient(string clientId, string clientSecret, Domains.Realm realm = null, params string[] redirectUrls)
        {
            var client = new Client
            {
                Id = Guid.NewGuid().ToString(),
                ClientId = clientId,
                ClientType = ClientTypes.WEBSITE,
                RedirectionUrls = redirectUrls,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
                ResponseTypes = new List<string> { AuthorizationCodeResponseTypeHandler.RESPONSE_TYPE },
                Secrets = new List<ClientSecret>
                {
                    ClientSecret.Create(clientSecret, HashAlgs.PLAINTEXT)
                }
            };
            if (realm == null) client.Realms.Add(Config.DefaultRealms.Master);
            else client.Realms.Add(realm);
            client.GrantTypes.Add(AuthorizationCodeHandler.GRANT_TYPE);
            client.TokenEndPointAuthMethod = OAuthClientSecretPostAuthenticationHandler.AUTH_METHOD;
            return new TraditionalWebsiteClientBuilder(client);
        }

        public static TraditionalWebsiteClientBuilder BuildSidAdmWebsite(string password, List<Scope> additionalScopes, params string[] redirectUrls)
        {
            var scopes = new List<Scope>
            {
                DefaultScopes.Role,
                DefaultScopes.OpenIdScope,
                DefaultScopes.Profile,
                DefaultScopes.Provisioning,
                DefaultScopes.Users,
                DefaultScopes.Workflows,
                DefaultScopes.Acrs,
                DefaultScopes.ConfigurationsScope,
                DefaultScopes.AuthenticationSchemeProviders,
                DefaultScopes.AuthenticationMethods,
                DefaultScopes.RegistrationWorkflows,
                DefaultScopes.ApiResources,
                DefaultScopes.Auditing,
                DefaultScopes.Scopes,
                DefaultScopes.CertificateAuthorities,
                DefaultScopes.Clients,
                DefaultScopes.Realms,
                DefaultScopes.Groups,
                DefaultScopes.WebsiteAdministratorRole,
                DefaultScopes.Forms,
                DefaultScopes.RecurringJobs,
                DefaultScopes.Migrations
            };
            scopes.AddRange(additionalScopes);
            return ClientBuilder.BuildTraditionalWebsiteClient(DefaultClients.SidAdminClientId, password, null, redirectUrls)
                .EnableClientGrantType()
                .SetRequestObjectEncryption()
                .AddAuthDataTypes("photo")
                .SetClientName("SimpleIdServer manager")
                .SetClientLogoUri("https://cdn.logo.com/hotlink-ok/logo-social.png")
                .AddScope(scopes.ToArray());
        }

        /// <summary>
        /// Build external authentication device client.
        /// CIBA is enabled.
        /// </summary>
        /// <returns></returns>
        public static ExternalDeviceClientBuilder BuildExternalAuthDeviceClient(string clientId, string subjectName, Domains.Realm realm = null)
        {
            var client = new Client
            {
                Id = Guid.NewGuid().ToString(),
                ClientId = clientId,
                ClientType = ClientTypes.EXTERNAL,
                TlsClientAuthSubjectDN = subjectName,
                TokenEndPointAuthMethod = OAuthClientTlsClientAuthenticationHandler.AUTH_METHOD,
                BCTokenDeliveryMode = Config.DefaultNotificationModes.Poll,
                BCAuthenticationRequestSigningAlg = SecurityAlgorithms.EcdsaSha256,
                IdTokenSignedResponseAlg = SecurityAlgorithms.EcdsaSha256,
                BCUserCodeParameter = false,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
                Secrets = new List<ClientSecret>
                {
                    ClientSecret.Create(Guid.NewGuid().ToString(), HashAlgs.PLAINTEXT)
                }
            };
            if (realm == null) client.Realms.Add(Config.DefaultRealms.Master);
            else client.Realms.Add(realm);
            client.GrantTypes.Add(CIBAHandler.GRANT_TYPE);
            return new ExternalDeviceClientBuilder(client);
        }

        /// <summary>
        /// Build a device client.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="realm"></param>
        /// <returns></returns>
        public static DeviceClientBuilder BuildDeviceClient(string clientId, string clientSecret, Domains.Realm realm = null)
        {
            var client = new Client
            {
                Id = Guid.NewGuid().ToString(),
                ClientId = clientId,
                ClientType = ClientTypes.DEVICE,
                TokenEndPointAuthMethod = OAuthClientSecretPostAuthenticationHandler.AUTH_METHOD,
                IdTokenSignedResponseAlg = SecurityAlgorithms.EcdsaSha256,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
                Secrets = new List<ClientSecret>
                {
                    ClientSecret.Create(clientSecret, HashAlgs.PLAINTEXT)
                }
            };
            if (realm == null) client.Realms.Add(Config.DefaultRealms.Master);
            else client.Realms.Add(realm);
            client.GrantTypes.Add(DeviceCodeHandler.GRANT_TYPE);
            return new DeviceClientBuilder(client);
        }

        /// <summary>
        /// Build mobile application.
        /// Authorization code + PKCE.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="redirectUrls"></param>
        /// <returns></returns>
        public static MobileClientBuilder BuildMobileApplication(string clientId, string clientSecret, Domains.Realm realm = null, params string[] redirectUrls)
        {
            var client = new Client
            {
                Id = Guid.NewGuid().ToString(),
                ClientId = clientId,
                IsPublic = true,
                ClientType = ClientTypes.MOBILE,
                RedirectionUrls = redirectUrls,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
                ResponseTypes = new List<string> { AuthorizationCodeResponseTypeHandler.RESPONSE_TYPE },
                Secrets = new List<ClientSecret>
                {
                    ClientSecret.Create(clientSecret, HashAlgs.PLAINTEXT)
                }
            };
            if (realm == null) client.Realms.Add(Config.DefaultRealms.Master);
            else client.Realms.Add(realm);
            client.GrantTypes.Add(AuthorizationCodeHandler.GRANT_TYPE);
            return new MobileClientBuilder(client);
        }

        /// <summary>
        /// Build client for user-agent based application for example : SPA, angular etc...
        /// Authorization code + PKCE.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="redirectUrls"></param>
        /// <returns></returns>
        public static UserAgentClientBuilder BuildUserAgentClient(string clientId, string clientSecret, Domains.Realm realm = null, params string[] redirectUrls)
        {
            var client = new Client
            {
                Id = Guid.NewGuid().ToString(),
                ClientId = clientId,
                RedirectionUrls = redirectUrls,
                ClientType = ClientTypes.SPA,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
                IsPublic = true,
                ResponseTypes = new List<string> { AuthorizationCodeResponseTypeHandler.RESPONSE_TYPE },
                Secrets = new List<ClientSecret>
                {
                    ClientSecret.Create(clientSecret, HashAlgs.PLAINTEXT)
                }
            };
            if (realm == null) client.Realms.Add(Config.DefaultRealms.Master);
            else client.Realms.Add(realm);
            client.GrantTypes.Add(AuthorizationCodeHandler.GRANT_TYPE);
            return new UserAgentClientBuilder(client);
        }
    }
}
