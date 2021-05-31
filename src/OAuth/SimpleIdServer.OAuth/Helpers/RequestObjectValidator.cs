// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt.Exceptions;
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Jwt;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Helpers
{
    public class RequestObjectValidator : IRequestObjectValidator
    {
        private readonly IJwtParser _jwtParser;

        public RequestObjectValidator(IJwtParser jwtParser)
        {
            _jwtParser = jwtParser;
        }

        public virtual async Task<RequestObjectValidatorResult> Validate(string request, BaseClient oauthClient, CancellationToken cancellationToken, string errorCode = ErrorCodes.INVALID_REQUEST_OBJECT)
        {
            if (!_jwtParser.IsJwsToken(request) && !_jwtParser.IsJweToken(request))
            {
                throw new OAuthException(errorCode, ErrorMessages.INVALID_REQUEST_PARAMETER);
            }

            var jws = request;
            if (_jwtParser.IsJweToken(request))
            {
                jws = await _jwtParser.Decrypt(jws, cancellationToken);
                if (string.IsNullOrWhiteSpace(jws))
                {
                    throw new OAuthException(errorCode, ErrorMessages.INVALID_JWE_REQUEST_PARAMETER);
                }
            }

            JwsHeader header = null;
            try
            {
                header = _jwtParser.ExtractJwsHeader(jws);
            }
            catch (InvalidOperationException)
            {
                throw new OAuthException(errorCode, ErrorMessages.INVALID_JWS_REQUEST_PARAMETER);
            }

            JwsPayload jwsPayload;
            try
            {
                jwsPayload = await _jwtParser.Unsign(jws, oauthClient, errorCode);
            }
            catch (JwtException ex)
            {
                throw new OAuthException(errorCode, ex.Message);
            }

            return new RequestObjectValidatorResult(jwsPayload, header);
        }
    }
}
