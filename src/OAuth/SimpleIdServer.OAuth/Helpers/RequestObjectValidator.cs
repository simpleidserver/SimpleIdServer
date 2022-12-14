// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Domains;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Helpers
{
    public interface IRequestObjectValidator
    {
        Task<RequestObjectValidatorResult> Validate(string request, Client oauthClient, CancellationToken cancellationToken, string errorCode = ErrorCodes.INVALID_REQUEST_OBJECT);
    }

    public class RequestObjectValidator : IRequestObjectValidator
    {
        public virtual async Task<RequestObjectValidatorResult> Validate(string request, Client oauthClient, CancellationToken cancellationToken, string errorCode = ErrorCodes.INVALID_REQUEST_OBJECT)
        {
            /*
            if (!_jwtParser.IsJwsToken(request) && !_jwtParser.IsJweToken(request)) throw new OAuthException(errorCode, ErrorMessages.INVALID_REQUEST_PARAMETER);
            var jws = request;
            if (_jwtParser.IsJweToken(request))
            {
                jws = await _jwtParser.Decrypt(jws, cancellationToken);
                if (string.IsNullOrWhiteSpace(jws)) throw new OAuthException(errorCode, ErrorMessages.INVALID_JWE_REQUEST_PARAMETER);
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
            */
            return null;
        }
    }

    public class RequestObjectValidatorResult
    {
        /*
        public RequestObjectValidatorResult(JwsPayload jwsPayload, JwsHeader header)
        {
            JwsPayload = jwsPayload;
            JwsHeader = header;
        }

        public JwsPayload JwsPayload { get; private set; }
        public JwsHeader JwsHeader { get; private set; }
        */
    }
}
