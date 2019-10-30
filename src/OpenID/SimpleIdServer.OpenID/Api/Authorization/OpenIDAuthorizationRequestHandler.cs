// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SimpleIdServer.Jwt.Extensions;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Authorization;
using SimpleIdServer.OAuth.Api.Authorization.ResponseTypes;
using SimpleIdServer.OAuth.Api.Authorization.Validators;
using SimpleIdServer.OAuth.Api.Token.TokenProfiles;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Infrastructures;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenID.DTOs;
using SimpleIdServer.OpenID.Exceptions;
using SimpleIdServer.OpenID.Extensions;
using SimpleIdServer.OpenID.Options;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.Authorization
{
    public class OpenIDAuthorizationRequestHandler : AuthorizationRequestHandler
    {
        private readonly OpenIDHostOptions _openidHostOptions;

        public OpenIDAuthorizationRequestHandler(IEnumerable<IResponseTypeHandler> responseTypeHandlers, IEnumerable<IOAuthResponseMode> oauthResponseModes, IEnumerable<IAuthorizationRequestValidator> authorizationRequestValidators, IEnumerable<ITokenProfile> tokenProfiles, IAuthorizationRequestEnricher authorizationRequestEnricher, IOAuthClientQueryRepository oauthClientRepository, IOAuthUserQueryRepository oauthUserRepository, IHttpClientFactory httpClientFactory, IOptions<OpenIDHostOptions> options) : base(responseTypeHandlers, oauthResponseModes, authorizationRequestValidators, tokenProfiles, authorizationRequestEnricher, oauthClientRepository, oauthUserRepository, httpClientFactory)
        {
            _openidHostOptions = options.Value;
        }

        public override async Task<AuthorizationResponse> Handle(HandlerContext context)
        {
            try
            {
                var result = await base.BuildResponse(context);
                var display = context.Request.Data.GetDisplayFromAuthorizationRequest();
                if (!string.IsNullOrWhiteSpace(display))
                {
                    context.Response.Add(AuthorizationRequestParameters.Display, display);
                }

                var sessionState = BuildSessionState(context);
                if (!string.IsNullOrWhiteSpace(sessionState))
                {
                    context.Response.Add(AuthorizationRequestParameters.SessionState, sessionState);
                }

                return result;
            }
            catch(OAuthUserConsentRequiredException)
            {
                context.Request.Data.Remove(AuthorizationRequestParameters.Prompt);
                return new RedirectActionAuthorizationResponse("Index", "Consents", context.Request.Data);
            }
            catch (OAuthLoginRequiredException ex)
            {
                context.Request.Data.Remove(AuthorizationRequestParameters.Prompt);
                return new RedirectActionAuthorizationResponse("Index", "Authenticate", context.Request.Data, ex.Area);
            }
            catch (OAuthSelectAccountRequiredException)
            {
                context.Request.Data.Remove(AuthorizationRequestParameters.Prompt);
                return new RedirectActionAuthorizationResponse("Index", "Accounts", context.Request.Data);
            }
        }

        private string BuildSessionState(HandlerContext handlerContext)
        {
            var sessionId = Guid.NewGuid().ToString();
            if (handlerContext.Request.Cookies.ContainsKey(_openidHostOptions.SessionCookieName))
            {
                sessionId = handlerContext.Request.Cookies[_openidHostOptions.SessionCookieName];
            }
            else
            {
                handlerContext.Response.Cookies.Append(_openidHostOptions.SessionCookieName, sessionId, new CookieOptions
                {
                    HttpOnly = false,
                    SameSite = SameSiteMode.None
                });
            }

            var redirectUrl = handlerContext.Request.Data.GetRedirectUriFromAuthorizationRequest();
            var clientId = handlerContext.Client.ClientId;
            var salt = Guid.NewGuid().ToString();
            return BuildSessionState(redirectUrl, clientId, salt, sessionId);
        }

        public static string BuildSessionState(string redirectUrl, string clientId, string salt, string sessionId)
        {
            var uri = new Uri(redirectUrl);
            var origin = uri.Scheme + "://" + uri.Host;
            if (!uri.IsDefaultPort)
            {
                origin += ":" + uri.Port;
            }

            var payload = Encoding.UTF8.GetBytes($"{clientId}{origin}{sessionId}{salt}");
            byte[] hash;
            using (var sha = SHA256.Create())
            {
                hash = sha.ComputeHash(payload);
            }

            return hash.Base64EncodeBytes() + "." + salt;
        }
    }
}