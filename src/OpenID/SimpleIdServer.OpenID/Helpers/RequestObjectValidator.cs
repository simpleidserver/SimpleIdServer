// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt.Exceptions;
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.Jwt.Jws.Handlers;
using SimpleIdServer.OAuth;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.OpenID.Domains;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Helpers
{
    public class RequestObjectValidator : IRequestObjectValidator
    {
        private readonly IJwtParser _jwtParser;

        public RequestObjectValidator(IJwtParser jwtParser)
        {
            _jwtParser = jwtParser;
        }

        public virtual async Task<RequestObjectValidatorResult> Validate(string request, OpenIdClient openidClient, CancellationToken cancellationToken)
        {
            if (!_jwtParser.IsJwsToken(request) && !_jwtParser.IsJweToken(request))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_REQUEST_PARAMETER);
            }

            var jws = request;
            if (_jwtParser.IsJweToken(request))
            {
                jws = await _jwtParser.Decrypt(jws);
                if (string.IsNullOrWhiteSpace(jws))
                {
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_JWE_REQUEST_PARAMETER);
                }
            }

            JwsHeader header = null;
            try
            {
                header = _jwtParser.ExtractJwsHeader(jws);
            }
            catch (InvalidOperationException)
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_JWS_REQUEST_PARAMETER);
            }

            JwsPayload jwsPayload;
            try
            {
                jwsPayload = await _jwtParser.Unsign(jws, openidClient);
            }
            catch (JwtException ex)
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ex.Message);
            }

            return new RequestObjectValidatorResult(jwsPayload, header);
        }
    }
}
