// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.OAuth;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.Uma.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.Uma.Api.Token.Fetchers
{
    public class OpenIDClaimTokenFormat : IClaimTokenFormat
    {
        private readonly UMAHostOptions _umaHostOptions;
        private readonly IJwtParser _jwtParser;

        public OpenIDClaimTokenFormat(IOptions<UMAHostOptions> umaHostOptions, IJwtParser jwtParser)
        {
            _umaHostOptions = umaHostOptions.Value;
            _jwtParser = jwtParser;
        }

        public const string NAME = "http://openid.net/specs/openid-connect-core-1_0.html#IDToken";
        public string Name => NAME;

        public Task<ClaimTokenFormatFetcherResult> Fetch(HandlerContext context)
        {
            var claimToken = context.Request.Data.GetClaimToken();
            if (!_jwtParser.IsJwsToken(claimToken))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, UMAErrorMessages.BAD_CLAIM_TOKEN);
            }

            var payload = _jwtParser.Unsign(claimToken, _umaHostOptions.OpenIdJsonWebKeySignature);
            if (payload == null)
            {
                return null;
            }

            var sub = payload.First(c => c.Key == GetSubjectName());
            return Task.FromResult(new ClaimTokenFormatFetcherResult(sub.Value.ToString(), payload));
        }

        public string GetSubjectName()
        {
            return Jwt.Constants.UserClaims.Subject;
        }
    }
}
