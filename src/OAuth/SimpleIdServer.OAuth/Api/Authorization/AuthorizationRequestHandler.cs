// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Api.Authorization.ResponseTypes;
using SimpleIdServer.OAuth.Api.Authorization.Validators;
using SimpleIdServer.OAuth.Api.Token.TokenProfiles;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Infrastructures;
using SimpleIdServer.OAuth.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Authorization
{
    public interface IAuthorizationRequestHandler
    {
        Task<AuthorizationResponse> Handle(HandlerContext context, CancellationToken token);
    }

    public class AuthorizationRequestHandler : IAuthorizationRequestHandler
    {
        private readonly IEnumerable<IResponseTypeHandler> _responseTypeHandlers;
        private readonly IEnumerable<IOAuthResponseMode> _oauthResponseModes;
        private readonly IEnumerable<IAuthorizationRequestValidator> _authorizationRequestValidators;
        private readonly IEnumerable<ITokenProfile> _tokenProfiles;
        private readonly IAuthorizationRequestEnricher _authorizationRequestEnricher;
        private readonly IOAuthClientQueryRepository _oauthClientRepository;
        private readonly IOAuthUserQueryRepository _oauthUserRepository;
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthorizationRequestHandler(IEnumerable<IResponseTypeHandler> responseTypeHandlers,
            IEnumerable<IOAuthResponseMode> oauthResponseModes, IEnumerable<IAuthorizationRequestValidator> authorizationRequestValidators,
            IEnumerable<ITokenProfile> tokenProfiles, IAuthorizationRequestEnricher authorizationRequestEnricher, IOAuthClientQueryRepository oauthClientRepository, IOAuthUserQueryRepository oauthUserRepository,
            IHttpClientFactory httpClientFactory)
        {
            _responseTypeHandlers = responseTypeHandlers;
            _oauthResponseModes = oauthResponseModes;
            _authorizationRequestValidators = authorizationRequestValidators;
            _tokenProfiles = tokenProfiles;
            _authorizationRequestEnricher = authorizationRequestEnricher;
            _oauthClientRepository = oauthClientRepository;
            _oauthUserRepository = oauthUserRepository;
            _httpClientFactory = httpClientFactory;
        }

        public virtual async Task<AuthorizationResponse> Handle(HandlerContext context, CancellationToken token)
        {
            try
            {
                return await BuildResponse(context, token);
            }
            catch (OAuthUserConsentRequiredException)
            {
                return new RedirectActionAuthorizationResponse("Index", "Consents", context.Request.Data);
            }
            catch (OAuthLoginRequiredException ex)
            {
                return new RedirectActionAuthorizationResponse("Index", "Authenticate", context.Request.Data, ex.Area);
            }
        }

        protected async Task<AuthorizationResponse> BuildResponse(HandlerContext context, CancellationToken cancellationToken)
        {
            var requestedResponseTypes = context.Request.Data.GetResponseTypesFromAuthorizationRequest();
            if (!requestedResponseTypes.Any())
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, AuthorizationRequestParameters.ResponseType));
            }

            var responseTypeHandlers = _responseTypeHandlers.Where(r => requestedResponseTypes.Contains(r.ResponseType));
            var unsupportedResponseType = requestedResponseTypes.Where(r => !_responseTypeHandlers.Any(rh => rh.ResponseType == r));
            if (unsupportedResponseType.Any())
            {
                throw new OAuthException(ErrorCodes.UNSUPPORTED_RESPONSE_TYPE, string.Format(ErrorMessages.MISSING_RESPONSE_TYPES, string.Join(" ", unsupportedResponseType)));
            }

            context.SetClient(await Validate(context.Request.Data, cancellationToken));
            context.SetUser(await _oauthUserRepository.FindOAuthUserByLogin(context.Request.UserSubject, cancellationToken));
            foreach (var validator in _authorizationRequestValidators)
            {
                await validator.Validate(context);
            }

            var state = context.Request.Data.GetStateFromAuthorizationRequest();
            var redirectUri = context.Request.Data.GetRedirectUriFromAuthorizationRequest();
            if (!string.IsNullOrWhiteSpace(state))
            {
                context.Response.Add(AuthorizationResponseParameters.State, state);
            }

            _authorizationRequestEnricher.Enrich(context);
            if (!context.Client.RedirectionUrls.Contains(redirectUri))
            {
                redirectUri = context.Client.RedirectionUrls.First();
            }

            foreach (var responseTypeHandler in responseTypeHandlers)
            {
                await responseTypeHandler.Enrich(context, cancellationToken);
            }

            _tokenProfiles.First(t => t.Profile == context.Client.PreferredTokenProfile).Enrich(context);
            return new RedirectURLAuthorizationResponse(redirectUri, context.Response.Parameters);
        }

        private async Task<OAuthClient> Validate(JObject jObj, CancellationToken cancellationToken)
        {
            var responseTypes = jObj.GetResponseTypesFromAuthorizationRequest();
            var responseMode = jObj.GetResponseModeFromAuthorizationRequest();
            var clientId = jObj.GetClientIdFromAuthorizationRequest();
            var state = jObj.GetStateFromAuthorizationRequest();
            var redirectUri = jObj.GetRedirectUriFromAuthorizationRequest();
            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, AuthorizationRequestParameters.ClientId));
            }

            var client = await _oauthClientRepository.FindOAuthClientById(clientId, cancellationToken);
            if (client == null)
            {
                throw new OAuthException(ErrorCodes.INVALID_CLIENT, string.Format(ErrorMessages.UNKNOWN_CLIENT, clientId));
            }

            var redirectionUrls = await client.GetRedirectionUrls(_httpClientFactory);
            if (!string.IsNullOrWhiteSpace(redirectUri) && !redirectionUrls.Contains(redirectUri))
            {
                throw new OAuthExceptionBadRequestURIException(redirectUri);
            }

            if (!string.IsNullOrWhiteSpace(responseMode) && !_oauthResponseModes.Any(o => o.ResponseMode == responseMode))
            {
                throw new OAuthException(ErrorCodes.UNSUPPORTED_RESPONSE_MODE, string.Format(ErrorMessages.BAD_RESPONSE_MODE, responseMode));
            }

            var unsupportedResponseTypes = responseTypes.Where(t => !client.ResponseTypes.Contains(t));
            if (unsupportedResponseTypes.Any())
            {
                throw new OAuthException(ErrorCodes.UNSUPPORTED_RESPONSE_TYPE, string.Format(ErrorMessages.BAD_RESPONSE_TYPES_CLIENT, string.Join(",", unsupportedResponseTypes)));
            }

            return client;
        }
    }
}