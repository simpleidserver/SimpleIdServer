// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Jwt;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.ClaimTokenFormats
{
    public interface IClaimTokenFormat
    {
        string Name { get; }
        Task<ClaimTokenFormatFetcherResult> Fetch(string claimToken, CancellationToken cancellationToken);
    }

    public class ClaimTokenFormatFetcherResult
    {
        public string UserNameIdentifier { get; set; }
        public IEnumerable<Claim> Claims { get; set; }
    }

    public class OpenIDClaimTokenFormat : IClaimTokenFormat
    {
        private readonly IJwtBuilder _jwtBuilder;

        public OpenIDClaimTokenFormat(IJwtBuilder jwtBuilder)
        {
            _jwtBuilder = jwtBuilder;
        }

        public const string NAME = "http://openid.net/specs/openid-connect-core-1_0.html#IDToken";
        public string Name => NAME;

        public Task<ClaimTokenFormatFetcherResult> Fetch(string claimToken, CancellationToken cancellationToken)
        {
            var extractionResult = _jwtBuilder.ReadSelfIssuedJsonWebToken(claimToken);
            if (extractionResult.Error != null)
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, extractionResult.Error);
            return Task.FromResult(new ClaimTokenFormatFetcherResult { UserNameIdentifier = extractionResult.Jwt.Subject, Claims = extractionResult.Jwt.Claims });
        }
    }
}
