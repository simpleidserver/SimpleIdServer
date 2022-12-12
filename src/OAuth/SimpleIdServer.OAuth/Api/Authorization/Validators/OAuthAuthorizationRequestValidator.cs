// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Authorization.Validators
{
    public class OAuthAuthorizationRequestValidator : IAuthorizationRequestValidator
    {
        private readonly IUserConsentFetcher _userConsentFetcher;
        private readonly IEnumerable<IOAuthResponseMode> _oauthResponseModes;
        private readonly IClientHelper _clientHelper;

        public OAuthAuthorizationRequestValidator(
            IUserConsentFetcher userConsentFetcher,
            IEnumerable<IOAuthResponseMode> oauthResponseModes,
            IClientHelper clientHelper)
        {
            _userConsentFetcher = userConsentFetcher;
            _oauthResponseModes = oauthResponseModes;
            _clientHelper = clientHelper;
        }

        public virtual async Task Validate(HandlerContext context, CancellationToken cancellationToken)
        {
            if (context.User == null)
            {
                throw new OAuthLoginRequiredException();
            }

            await CommonValidate(context, cancellationToken);
            var scopes = context.Request.RequestData.GetScopesFromAuthorizationRequest();
            var unsupportedScopes = scopes.Where(s => !context.Client.Scopes.Any(sc => sc.Scope == s));
            if (unsupportedScopes.Any())
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNSUPPORTED_SCOPES, string.Join(",", unsupportedScopes)));
            }

            if (context.Client.IsConsentDisabled) return;
            var consent = _userConsentFetcher.FetchFromAuthorizationRequest(context.User, context.Request.RequestData);
            if (consent == null)
            {
                throw new OAuthUserConsentRequiredException();
            }
        }

        protected async Task CommonValidate(HandlerContext context, CancellationToken cancellationToken)
        {
            var client = context.Client;
            var redirectUri = context.Request.RequestData.GetRedirectUriFromAuthorizationRequest();
            var responseTypes = context.Request.RequestData.GetResponseTypesFromAuthorizationRequest();
            var responseMode = context.Request.RequestData.GetResponseModeFromAuthorizationRequest();
            var unsupportedResponseTypes = responseTypes.Where(t => !client.ResponseTypes.Contains(t));
            var redirectionUrls = await _clientHelper.GetRedirectionUrls(client, cancellationToken);
            if (!string.IsNullOrWhiteSpace(redirectUri) && !redirectionUrls.Contains(redirectUri))
            {
                throw new OAuthExceptionBadRequestURIException(redirectUri);
            }

            if (!string.IsNullOrWhiteSpace(responseMode) && !_oauthResponseModes.Any(o => o.ResponseMode == responseMode))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.BAD_RESPONSE_MODE, responseMode));
            }

            if (unsupportedResponseTypes.Any())
            {
                throw new OAuthException(ErrorCodes.UNSUPPORTED_RESPONSE_TYPE, string.Format(ErrorMessages.BAD_RESPONSE_TYPES_CLIENT, string.Join(",", unsupportedResponseTypes)));
            }
        }
    }
}