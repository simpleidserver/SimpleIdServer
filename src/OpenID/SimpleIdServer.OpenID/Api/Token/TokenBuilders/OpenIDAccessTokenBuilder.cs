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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SimpleIdServer.Jwt.Constants;

namespace SimpleIdServer.OpenID.Api.Token.TokenBuilders
{
    public class OpenIDAccessTokenBuilder : AccessTokenBuilder
    {
        public OpenIDAccessTokenBuilder(IGrantedTokenHelper grantedTokenHelper, IJwtBuilder jwtBuilder, IOptions<OAuthHostOptions> options) : base(grantedTokenHelper, jwtBuilder, options) { }

        public override async Task Build(IEnumerable<string> scopes, HandlerContext handlerContext, CancellationToken cancellationToken)
        {
            var claimParameters = handlerContext.Request.Data.GetClaimsFromAuthorizationRequest();
            var jwsPayload = BuildOpenIdPayload(scopes, claimParameters, handlerContext);
            await SetResponse(handlerContext, jwsPayload, cancellationToken);
        }

        public override async Task Refresh(JObject previousRequest, HandlerContext currentContext, CancellationToken cancellationToken)
        {
            var scopes = previousRequest.GetScopesFromAuthorizationRequest();
            var claimParameters = previousRequest.GetClaimsFromAuthorizationRequest();
            var jwsPayload = BuildOpenIdPayload(scopes, claimParameters, currentContext);
            await SetResponse(currentContext, jwsPayload, cancellationToken);
        }

        protected virtual JwsPayload BuildOpenIdPayload(IEnumerable<string> scopes, IEnumerable<AuthorizationRequestClaimParameter> claimParameters, HandlerContext handlerContext)
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

            if (claimParameters != null)
            {
                var jObj = new JObject();
                var filtered = claimParameters.Where(cl => cl.Type == AuthorizationRequestClaimTypes.UserInfo);
                foreach (var record in filtered)
                {
                    var value = new JObject();
                    if (record.IsEssential)
                    {
                        value.Add(ClaimsParameter.Essential, true);
                    }

                    if (record.Values != null && record.Values.Any())
                    {
                        if (record.Values.Count() == 1)
                        {
                            value.Add(ClaimsParameter.Value, record.Values.First());
                        }
                        else
                        {
                            value.Add(ClaimsParameter.Values, new JArray(record.Values));
                        }
                    }

                    jObj.Add(record.Name, value);
                }

                if (filtered.Any())
                {
                    jwsPayload.Add(AuthorizationRequestParameters.Claims, jObj);
                }
            }

            return jwsPayload;
        }
    }
}