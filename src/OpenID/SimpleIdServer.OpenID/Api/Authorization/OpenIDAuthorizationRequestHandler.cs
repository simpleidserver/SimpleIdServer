// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Authorization;
using SimpleIdServer.OAuth.Api.Authorization.ResponseTypes;
using SimpleIdServer.OAuth.Api.Authorization.Validators;
using SimpleIdServer.OAuth.Api.Token.TokenProfiles;
using SimpleIdServer.OAuth.Domains.Clients;
using SimpleIdServer.OAuth.Domains.Users;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Infrastructures;
using SimpleIdServer.OpenID.DTOs;
using SimpleIdServer.OpenID.Exceptions;
using SimpleIdServer.OpenID.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.Authorization
{
    public class OpenIDAuthorizationRequestHandler : AuthorizationRequestHandler
    {
        public OpenIDAuthorizationRequestHandler(IEnumerable<IResponseTypeHandler> responseTypeHandlers, IEnumerable<IOAuthResponseMode> oauthResponseModes, IEnumerable<IAuthorizationRequestValidator> authorizationRequestValidators, IEnumerable<ITokenProfile> tokenProfiles, IAuthorizationRequestEnricher authorizationRequestEnricher, IOAuthClientQueryRepository oauthClientRepository, IOAuthUserQueryRepository oauthUserRepository, IHttpClientFactory httpClientFactory) : base(responseTypeHandlers, oauthResponseModes, authorizationRequestValidators, tokenProfiles, authorizationRequestEnricher, oauthClientRepository, oauthUserRepository, httpClientFactory)
        {
        }

        public override async Task<AuthorizationResponse> Handle(HandlerContext context)
        {
            try
            {
                var result = await base.BuildResponse(context);
                var display = context.Request.QueryParameters.GetDisplayFromAuthorizationRequest();
                if (!string.IsNullOrWhiteSpace(display))
                {
                    context.Response.Add(AuthorizationRequestParameters.Display, display);
                }

                return result;
            }
            catch(OAuthUserConsentRequiredException)
            {
                context.Request.QueryParameters.Remove(AuthorizationRequestParameters.Prompt);
                return new RedirectActionAuthorizationResponse("Index", "Consents", context.Request.QueryParameters);
            }
            catch (OAuthLoginRequiredException ex)
            {
                context.Request.QueryParameters.Remove(AuthorizationRequestParameters.Prompt);
                return new RedirectActionAuthorizationResponse("Index", "Authenticate", context.Request.QueryParameters, ex.Area);
            }
            catch (OAuthSelectAccountRequiredException)
            {
                context.Request.QueryParameters.Remove(AuthorizationRequestParameters.Prompt);
                return new RedirectActionAuthorizationResponse("Index", "Accounts", context.Request.QueryParameters);
            }
        }
    }
}