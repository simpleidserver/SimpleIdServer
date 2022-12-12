// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Domains;
using SimpleIdServer.OAuth.Api.Token.PKCECodeChallengeMethods;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Authenticate.Handlers
{
    public class OAuthPKCEAuthenticationHandler : IOAuthClientAuthenticationHandler
    {
        private readonly IGrantedTokenHelper _grantedTokenHelper;
        private readonly IEnumerable<ICodeChallengeMethodHandler> _codeChallengeMethodHandlers;

        public OAuthPKCEAuthenticationHandler(IGrantedTokenHelper grantedTokenHelper, IEnumerable<ICodeChallengeMethodHandler> codeChallengeMethodHandlers)
        {
            _grantedTokenHelper = grantedTokenHelper;
            _codeChallengeMethodHandlers = codeChallengeMethodHandlers;
        }

        public const string AUTH_METHOD = "pkce";
        public string AuthMethod => AUTH_METHOD;

        public async Task<bool> Handle(AuthenticateInstruction authenticateInstruction, Client client, string expectedIssuer, CancellationToken cancellationToken, string errorCode = ErrorCodes.INVALID_CLIENT)
        {
            var codeVerifier = authenticateInstruction.RequestData.GetCodeVerifier();
            if (string.IsNullOrWhiteSpace(codeVerifier))
            {
                throw new OAuthException(errorCode, string.Format(ErrorMessages.MISSING_PARAMETER, TokenRequestParameters.CodeVerifier));
            }

            var code = authenticateInstruction.RequestData.GetAuthorizationCode();
            if (code == null)
            {
                return false;
            }

            var previousRequest = await _grantedTokenHelper.GetAuthorizationCode(code, cancellationToken);
            if (previousRequest == null)
            {
                return false;
            }

            var codeChallenge = previousRequest.GetCodeChallengeFromAuthorizationRequest();
            var codeChallengeMethod = previousRequest.GetCodeChallengeMethodFromAuthorizationRequest();
            if (string.IsNullOrWhiteSpace(codeChallengeMethod))
            {
                codeChallengeMethod = PlainCodeChallengeMethodHandler.DEFAULT_NAME;
            }

            var codeChallengeMethodHandler = _codeChallengeMethodHandlers.First(c => c.Name == codeChallengeMethod);
            var newCodeChallenge = codeChallengeMethodHandler.Calculate(codeVerifier);
            if (newCodeChallenge != codeChallenge)
            {
                throw new OAuthException(errorCode, ErrorMessages.BAD_CODE_VERIFIER);
            }

            return true;
        }
    }
}