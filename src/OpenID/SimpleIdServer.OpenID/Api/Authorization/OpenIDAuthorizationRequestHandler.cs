// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Authorization;
using SimpleIdServer.OAuth.Api.Authorization.ResponseTypes;
using SimpleIdServer.OAuth.Api.Authorization.Validators;
using SimpleIdServer.OAuth.Api.Token.TokenProfiles;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.OpenID.DTOs;
using SimpleIdServer.OpenID.Exceptions;
using SimpleIdServer.OpenID.Options;
using SimpleIdServer.Store;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.Authorization
{
    public class OpenIDAuthorizationRequestHandler : AuthorizationRequestHandler
    {
        private readonly OpenIDHostOptions _options;

        public OpenIDAuthorizationRequestHandler(
            IOptions<OpenIDHostOptions> options,
            IOptions<OAuthHostOptions> opts,
            IEnumerable<IResponseTypeHandler> responseTypeHandlers, 
            IEnumerable<IAuthorizationRequestValidator> authorizationRequestValidators,
            IEnumerable<ITokenProfile> tokenProfiles, 
            IAuthorizationRequestEnricher authorizationRequestEnricher, 
            IClientRepository clientRepository,
            IUserRepository userRepository) : base(
                responseTypeHandlers, 
                authorizationRequestValidators, 
                tokenProfiles, 
                authorizationRequestEnricher, 
                clientRepository,
                userRepository,
                opts)
        {
            _options = options.Value;
        }

        public override async Task<AuthorizationResponse> Handle(HandlerContext context, CancellationToken token)
        {
            try
            {
                var result = await base.BuildResponse(context, token);
                var display = context.Request.RequestData.GetDisplayFromAuthorizationRequest();
                if (!string.IsNullOrWhiteSpace(display))
                    context.Response.Add(AuthorizationRequestParameters.Display, display);

                var sessionState = BuildSessionState(context);
                if (!string.IsNullOrWhiteSpace(sessionState))
                    context.Response.Add(AuthorizationRequestParameters.SessionState, sessionState);

                return result;
            }
            catch(OAuthUserConsentRequiredException ex)
            {
                context.Request.RequestData.Remove(AuthorizationRequestParameters.Prompt);
                return new RedirectActionAuthorizationResponse(ex.ActionName, ex.ControllerName, context.Request.OriginalRequestData);
            }
            catch (OAuthLoginRequiredException ex)
            {
                context.Request.RequestData.Remove(AuthorizationRequestParameters.Prompt);
                return new RedirectActionAuthorizationResponse("Index", "Authenticate", context.Request.OriginalRequestData, ex.Area, true, new List<string> { _options.SessionCookieName });
            }
            catch (OAuthSelectAccountRequiredException)
            {
                context.Request.RequestData.Remove(AuthorizationRequestParameters.Prompt);
                return new RedirectActionAuthorizationResponse("Index", "Accounts", context.Request.OriginalRequestData);
            }
        }

        private string BuildSessionState(HandlerContext handlerContext)
        {
            var session = handlerContext.User.ActiveSession;
            if (session == null)
                return null;

            var redirectUrl = handlerContext.Request.RequestData.GetRedirectUriFromAuthorizationRequest();
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