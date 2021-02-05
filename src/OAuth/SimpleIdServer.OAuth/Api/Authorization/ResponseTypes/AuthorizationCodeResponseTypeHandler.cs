// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Api.Token.Handlers;
using SimpleIdServer.OAuth.Api.Token.PKCECodeChallengeMethods;
using SimpleIdServer.OAuth.Authenticate.Handlers;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OAuth.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Authorization.ResponseTypes
{
    public class AuthorizationCodeResponseTypeHandler : IResponseTypeHandler
    {
        private readonly IGrantedTokenHelper _grantedTokenHelper;
        private readonly IEnumerable<ICodeChallengeMethodHandler> _codeChallengeMethodHandlers;
        private readonly OAuthHostOptions _options;

        public AuthorizationCodeResponseTypeHandler(IGrantedTokenHelper grantedTokenHelper, IEnumerable<ICodeChallengeMethodHandler> codeChallengeMethodHandlers, IOptions<OAuthHostOptions> options)
        {
            _grantedTokenHelper = grantedTokenHelper;
            _codeChallengeMethodHandlers = codeChallengeMethodHandlers;
            _options = options.Value;
        }

        public string GrantType => AuthorizationCodeHandler.GRANT_TYPE;
        public string ResponseType => RESPONSE_TYPE;
        public int Order => 1;
        public static string RESPONSE_TYPE = "code";

        public async Task Enrich(HandlerContext context, CancellationToken cancellationToken)
        {
            var dic = new JObject();
            foreach (var record in context.Request.Data)
            {
                dic.Add(record.Key, record.Value);
            }

            CheckPKCEParameters(context);
            var authCode = await _grantedTokenHelper.AddAuthorizationCode(dic, _options.AuthorizationCodeExpirationInSeconds, cancellationToken);
            context.Response.Add(AuthorizationResponseParameters.Code, authCode);
        }

        protected virtual void CheckPKCEParameters(HandlerContext handlerContext)
        {
            var codeChallenge = handlerContext.Request.Data.GetCodeChallengeFromAuthorizationRequest();
            var codeChallengeMethod = handlerContext.Request.Data.GetCodeChallengeMethodFromAuthorizationRequest();
            if (handlerContext.Client.TokenEndPointAuthMethod == OAuthPKCEAuthenticationHandler.AUTH_METHOD && string.IsNullOrWhiteSpace(codeChallenge))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, AuthorizationRequestParameters.CodeChallenge));
            }

            if (string.IsNullOrWhiteSpace(codeChallengeMethod))
            {
                codeChallengeMethod = PlainCodeChallengeMethodHandler.DEFAULT_NAME;
            }

            if (!_codeChallengeMethodHandlers.Any(c => c.Name == codeChallengeMethod))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.BAD_CODE_CHALLENGE_METHOD, codeChallengeMethod));
            }
        }
    }
}