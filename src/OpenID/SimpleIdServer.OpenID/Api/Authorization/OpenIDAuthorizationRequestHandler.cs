// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.Jwt.Extensions;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Authorization;
using SimpleIdServer.OAuth.Api.Authorization.ResponseTypes;
using SimpleIdServer.OAuth.Api.Authorization.Validators;
using SimpleIdServer.OAuth.Api.Token.TokenProfiles;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenID.DTOs;
using SimpleIdServer.OpenID.Exceptions;
using SimpleIdServer.OpenID.Extensions;
using SimpleIdServer.OpenID.Options;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.Authorization
{
    public class OpenIDAuthorizationRequestHandler : AuthorizationRequestHandler
    {
        private readonly OpenIDHostOptions _openidHostOptions;

        public OpenIDAuthorizationRequestHandler(
            IEnumerable<IResponseTypeHandler> responseTypeHandlers, 
            IEnumerable<IAuthorizationRequestValidator> authorizationRequestValidators,
            IEnumerable<ITokenProfile> tokenProfiles, 
            IAuthorizationRequestEnricher authorizationRequestEnricher, 
            IOAuthClientQueryRepository oauthClientRepository,
            IOAuthUserQueryRepository oauthUserRepository, 
            IOptions<OpenIDHostOptions> options) : base(
                responseTypeHandlers, 
                authorizationRequestValidators, 
                tokenProfiles, 
                authorizationRequestEnricher, 
                oauthClientRepository, 
                oauthUserRepository)
        {
            _openidHostOptions = options.Value;
        }

        public override async Task<AuthorizationResponse> Handle(HandlerContext context, CancellationToken token)
        {
            try
            {
                var result = await base.BuildResponse(context, token);
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
            catch(OAuthUserConsentRequiredException ex)
            {
                context.Request.Data.Remove(AuthorizationRequestParameters.Prompt);
                return new RedirectActionAuthorizationResponse(ex.ActionName, ex.ControllerName, context.Request.Data);
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
            var session = handlerContext.User.GetActiveSession();
            var redirectUrl = handlerContext.Request.Data.GetRedirectUriFromAuthorizationRequest();
            var clientId = handlerContext.Client.ClientId;
            var salt = Guid.NewGuid().ToString();
            return BuildSessionState(redirectUrl, clientId, salt, session.SessionId);
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