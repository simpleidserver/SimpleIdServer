// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Token.TokenBuilders;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.OpenID.DTOs;
using SimpleIdServer.OpenID.Extensions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static SimpleIdServer.Jwt.Constants;

namespace SimpleIdServer.OpenID.Api.Token.TokenBuilders
{
    public class OpenIDAccessTokenBuilder : AccessTokenBuilder
    {
        private readonly IClaimsJwsPayloadEnricher _claimsJwsPayloadEnricher;

        public OpenIDAccessTokenBuilder(IClaimsJwsPayloadEnricher claimsJwsPayloadEnricher,
            IGrantedTokenHelper grantedTokenHelper, 
            IJwtBuilder jwtBuilder, 
            IOptions<OAuthHostOptions> options) : base(grantedTokenHelper, jwtBuilder, options) {
            _claimsJwsPayloadEnricher = claimsJwsPayloadEnricher;
        }

        public override async Task Build(IEnumerable<string> scopes, HandlerContext handlerContext, CancellationToken cancellationToken)
        {
            var claimParameters = handlerContext.Request.Data.GetClaimsFromAuthorizationRequest();
            var jwsPayload = await BuildOpenIdPayload(scopes, claimParameters, handlerContext.Request.Data, handlerContext, cancellationToken);
            await SetResponse(handlerContext, jwsPayload, cancellationToken);
        }

        public override async Task Refresh(JObject previousRequest, HandlerContext currentContext, CancellationToken cancellationToken)
        {
            var scopes = previousRequest.GetScopesFromAuthorizationRequest();
            var claimParameters = previousRequest.GetClaimsFromAuthorizationRequest();
            var jwsPayload = await BuildOpenIdPayload(scopes, claimParameters, previousRequest, currentContext, cancellationToken);
            await SetResponse(currentContext, jwsPayload, cancellationToken);
        }

        protected virtual Task<JwsPayload> BuildOpenIdPayload(IEnumerable<string> scopes, IEnumerable<AuthorizationRequestClaimParameter> claimParameters, JObject queryParameters, HandlerContext handlerContext, CancellationToken cancellationToken)
        {
            var jwsPayload = BuildPayload(scopes, handlerContext);
            if (handlerContext.User != null)
            {
                jwsPayload.Add(UserClaims.Subject, handlerContext.User.Id);
                if (handlerContext.User.AuthenticationTime != null)
                {
                    jwsPayload.Add(OAuthClaims.AuthenticationTime, handlerContext.User.AuthenticationTime.Value.ConvertToUnixTimestamp());
                }
            }

            _claimsJwsPayloadEnricher.EnrichWithClaimsParameter(jwsPayload, claimParameters);
            return Task.FromResult(jwsPayload);
        }
    }
}