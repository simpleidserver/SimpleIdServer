// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.OpenID.Api.Token.TokenBuilders;
using SimpleIdServer.OpenID.DTOs;
using SimpleIdServer.OpenID.Extensions;
using SimpleIdServer.OpenID.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenBankingApi.Api.Token.TokenBuilders
{
    public class OpenBankingApiAccessTokenBuilder: OpenIDAccessTokenBuilder
    {
        private readonly IBCAuthorizeRepository _bcAuthorizeRepository;
        private readonly OpenBankingApiOptions _options;

        public OpenBankingApiAccessTokenBuilder(
            IBCAuthorizeRepository bcAuthorizeRepository,
            IOptions<OpenBankingApiOptions> openbankingOptions,
            IClaimsJwsPayloadEnricher claimsJwsPayloadEnricher,
            IGrantedTokenHelper grantedTokenHelper,
            IJwtBuilder jwtBuilder,
            IOptions<OAuthHostOptions> options) : base(claimsJwsPayloadEnricher, grantedTokenHelper, jwtBuilder, options)
        {
            _bcAuthorizeRepository = bcAuthorizeRepository;
            _options = openbankingOptions.Value;
        }

        protected override async Task<JwsPayload> BuildOpenIdPayload(IEnumerable<string> scopes, IEnumerable<AuthorizationRequestClaimParameter> claimParameters, HandlerContext handlerContext, CancellationToken cancellationToken)
        {
            var result = await base.BuildOpenIdPayload(scopes, claimParameters, handlerContext, cancellationToken);
            var authRequestId = handlerContext.Request.Data.GetAuthRequestId();
            if (!string.IsNullOrWhiteSpace(authRequestId))
            {
                var authorize = await _bcAuthorizeRepository.Get(authRequestId, cancellationToken);
                if (authorize != null && authorize.Permissions.Any())
                {
                    var firstPermission = authorize.Permissions.First();
                    if (result.ContainsKey(_options.OpenBankingApiConsentClaimName))
                    {
                        result[_options.OpenBankingApiConsentClaimName] = firstPermission.ConsentId;
                    }
                    else
                    {
                        result.Add(_options.OpenBankingApiConsentClaimName, firstPermission.ConsentId);
                    }
                }
            }

            return result;
        }
    }
}
