// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Authorization.Validators
{
    public class OAuthAuthorizationRequestValidator : IAuthorizationRequestValidator
    {
        private readonly IUserConsentFetcher _userConsentFetcher;

        public OAuthAuthorizationRequestValidator(IUserConsentFetcher userConsentFetcher)
        {
            _userConsentFetcher = userConsentFetcher;
        }

        public Task Validate(HandlerContext context, CancellationToken cancellationToken)
        {
            if (context.User == null)
            {
                throw new OAuthLoginRequiredException();
            }

            var scopes = context.Request.Data.GetScopesFromAuthorizationRequest();
            var unsupportedScopes = scopes.Where(s => !context.Client.AllowedScopes.Any(sc => sc.Name == s));
            if (unsupportedScopes.Any())
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNSUPPORTED_SCOPES, string.Join(",", unsupportedScopes)));
            }

            var consent = _userConsentFetcher.FetchFromAuthorizationRequest(context.User, context.Request.Data);
            if (consent == null)
            {
                throw new OAuthUserConsentRequiredException();
            }

            return Task.FromResult(0);
        }
    }
}