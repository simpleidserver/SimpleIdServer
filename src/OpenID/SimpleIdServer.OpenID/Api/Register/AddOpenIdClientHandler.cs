// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt.Jwe.EncHandlers;
using SimpleIdServer.Jwt.Jws.Handlers;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Register.Handlers;
using SimpleIdServer.OAuth.Api.Register.Validators;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Infrastructures;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.Extensions;
using SimpleIdServer.OpenID.Options;
using System.Linq;

namespace SimpleIdServer.OpenID.Api.Register
{
    /// <summary>
    /// https://openid.net/specs/openid-connect-registration-1_0.html
    /// </summary>
    public class AddOpenIdClientHandler : AddOAuthClientHandler
    {
        private readonly OpenIDHostOptions _openIDHostOptions;

        public AddOpenIdClientHandler(
            IOAuthClientQueryRepository oauthClientQueryRepository,
            IOAuthClientCommandRepository oAuthClientCommandRepository,
            IJwtParser jwtParser,
            IHttpClientFactory httpClientFactory,
            IOAuthClientValidator oauthClientValidator,
            IOptions<OAuthHostOptions> oauthHostOptions,
            IOptions<OpenIDHostOptions> openidHostOptions) : base(oauthClientQueryRepository, oAuthClientCommandRepository, jwtParser, httpClientFactory, oauthClientValidator, oauthHostOptions)
        {
            _openIDHostOptions = openidHostOptions.Value;
        }

        protected override OAuthClient ExtractClient(HandlerContext handlerContext)
        {
            var openIdClient = handlerContext.Request.Data.ToDomain();
            EnrichOpenIdClient(openIdClient, handlerContext);
            return openIdClient;
        }

        protected void EnrichOpenIdClient(OpenIdClient openidClient, HandlerContext handlerContext)
        {
            EnrichClient(openidClient);
            SetDefaultApplicationType(openidClient);
            SetDefaultSubjectType(openidClient);
            SetDefaultIdTokenResponseAlg(openidClient);
            SetDefaultMaxAge(openidClient);
            SetDefaultIdTokenEncryptedResponseAlg(openidClient);
            SetUserInfoEncryptedResponseAlg(openidClient);
            SetRequestObjectEncryptionAlg(openidClient);
            SetBCNotification(openidClient, handlerContext);
            SetFrontChannel(openidClient, handlerContext);
        }

        protected override JObject BuildResponse(OAuthClient oauthClient, string issuer)
        {
            var openidClient = oauthClient as OpenIdClient;
            return openidClient.ToDto(issuer);
        }

        protected override void SetDefaultScopes(OAuthClient oauthClient)
        {
            base.SetDefaultScopes(oauthClient);
            var scope = oauthClient.AllowedScopes.FirstOrDefault(s => s.Name == SIDOpenIdConstants.StandardScopes.OpenIdScope.Name);
            if (scope == null)
            {
                var scopes = oauthClient.AllowedScopes.ToList();
                scopes.Add(SIDOpenIdConstants.StandardScopes.OpenIdScope);
                oauthClient.AllowedScopes = scopes;
            }
        }

        protected virtual void SetDefaultApplicationType(OpenIdClient openidClient)
        {
            if (string.IsNullOrWhiteSpace(openidClient.ApplicationType))
            {
                openidClient.ApplicationType = "web";
            }
        }

        protected virtual void SetDefaultSubjectType(OpenIdClient openidClient)
        {
            if (string.IsNullOrWhiteSpace(openidClient.SubjectType))
            {
                openidClient.SubjectType = _openIDHostOptions.DefaultSubjectType;
            }
        }

        protected virtual void SetDefaultIdTokenResponseAlg(OpenIdClient openidClient)
        {
            if (string.IsNullOrWhiteSpace(openidClient.IdTokenSignedResponseAlg))
            {
                openidClient.IdTokenSignedResponseAlg = RSA256SignHandler.ALG_NAME;
            }
        }

        protected virtual void SetDefaultMaxAge(OpenIdClient openidClient)
        {
            if (openidClient.DefaultMaxAge == null)
            {
                openidClient.DefaultMaxAge = _openIDHostOptions.DefaultMaxAge;
            }
        }

        protected virtual void SetDefaultIdTokenEncryptedResponseAlg(OpenIdClient openidClient)
        {
            if (!string.IsNullOrWhiteSpace(openidClient.IdTokenEncryptedResponseAlg) && string.IsNullOrWhiteSpace(openidClient.IdTokenEncryptedResponseEnc))
            {
                openidClient.IdTokenEncryptedResponseEnc = A128CBCHS256EncHandler.ENC_NAME;
            }
        }

        protected virtual void SetUserInfoEncryptedResponseAlg(OpenIdClient openidClient)
        {
            if (!string.IsNullOrWhiteSpace(openidClient.UserInfoEncryptedResponseAlg) && string.IsNullOrWhiteSpace(openidClient.UserInfoEncryptedResponseEnc))
            {
                openidClient.UserInfoEncryptedResponseEnc = A128CBCHS256EncHandler.ENC_NAME;
            }
        }

        protected virtual void SetRequestObjectEncryptionAlg(OpenIdClient openidClient)
        {
            if (!string.IsNullOrWhiteSpace(openidClient.RequestObjectEncryptionAlg) && string.IsNullOrWhiteSpace(openidClient.RequestObjectEncryptionEnc))
            {
                openidClient.RequestObjectEncryptionEnc = _openIDHostOptions.DefaultRequestObjectEncyptionEnc;
            }
        }

        protected virtual void SetBCNotification(OpenIdClient openidClient, HandlerContext handlerContext)
        {
            openidClient.BCTokenDeliveryMode = handlerContext.Request.Data.GetBCTokenDeliveryMode();
            openidClient.BCClientNotificationEndpoint = handlerContext.Request.Data.GetBCClientNotificationEndpoint();
            openidClient.BCAuthenticationRequestSigningAlg = handlerContext.Request.Data.GetBCAuthenticationRequestSigningAlg();
            openidClient.BCUserCodeParameter = handlerContext.Request.Data.GetBCUserCodeParameter();
        }

        protected virtual void SetFrontChannel(OpenIdClient openidClient, HandlerContext handlerContext)
        {
            openidClient.FrontChannelLogoutUri = handlerContext.Request.Data.GetFrontChannelLogoutUri();
            openidClient.FrontChannelLogoutSessionRequired = handlerContext.Request.Data.GetFrontChannelLogoutSessionRequired();
        }
    }
}
