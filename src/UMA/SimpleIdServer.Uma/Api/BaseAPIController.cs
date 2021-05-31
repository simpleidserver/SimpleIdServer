// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.Uma.DTOs;
using SimpleIdServer.Uma.Persistence;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Uma.Api
{
    public class BaseAPIController : Controller
    {
        private readonly IJwtParser _jwtParser;
        private readonly UMAHostOptions _umaHostOptions;

        public BaseAPIController(IJwtParser jwtParser, IOptions<UMAHostOptions> umaHostoptions)
        {
            _jwtParser = jwtParser;
            _umaHostOptions = umaHostoptions.Value;
        }

        protected SearchRequestParameter ExtractSearchRequestParameter()
        {
            var result = new SearchRequestParameter();
            EnrichSearchRequestParameter(result);
            return result;
        }

        protected void EnrichSearchRequestParameter(SearchRequestParameter parameter)
        {
            var query = Request.Query.ToJObject();
            var count = query.GetInt(UMASearchRequestNames.Count);
            var startIndex = query.GetInt(UMASearchRequestNames.StartIndex);
            var sortKey = query.GetStr(UMASearchRequestNames.SortKey);
            var sortOrder = query.GetStr(UMASearchRequestNames.SortOrder);
            if (count != null)
            {
                parameter.Count = count.Value;
            }

            if (startIndex != null)
            {
                parameter.StartIndex = startIndex.Value;
            }

            if (!string.IsNullOrWhiteSpace(sortKey))
            {
                parameter.SortKey = sortKey;
            }

            if (!string.IsNullOrWhiteSpace(sortOrder))
            {
                if (sortOrder.Equals("asc", StringComparison.InvariantCultureIgnoreCase))
                {
                    parameter.SortOrder = SearchRequestOrders.ASC;
                }
                else if (sortOrder.Equals("desc", StringComparison.InvariantCultureIgnoreCase))
                {
                    parameter.SortOrder = SearchRequestOrders.DESC;
                }
            }
        }

        protected async Task<IActionResult> CallOperationWithAuthenticatedUser(Func<string, JwsPayload, Task<IActionResult>> callback)
        {
            var payload = ExtractOPENIDTokenPayload();
            if (payload == null)
            {
                return new UnauthorizedResult();
            }

            if (!payload.ContainsKey(Jwt.Constants.UserClaims.Subject))
            {
                return new UnauthorizedResult();
            }

            var subject = payload.First(s => s.Key == Jwt.Constants.UserClaims.Subject).Value.ToString();
            return await callback(subject, payload);
        }

        protected async Task<bool> IsPATAuthorized(CancellationToken cancellationToken)
        {
            var payload = await ExtractPATTokenPayload(cancellationToken);
            if(payload == null)
            {
                return false;
            }

            return payload.GetScopes().Contains(UMAConstants.StandardUMAScopes.UmaProtection.Name);
        }

        private JwsPayload ExtractOPENIDTokenPayload()
        {
            var token = ExtractAuthorizationValue();
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }

            var isJwsToken = _jwtParser.IsJwsToken(token);
            if (!isJwsToken)
            {
                return null;
            }

            return _jwtParser.Unsign(token, _umaHostOptions.OpenIdJsonWebKeySignature);
        }

        private async Task<JwsPayload> ExtractPATTokenPayload(CancellationToken cancellationToken)
        {
            var token = ExtractAuthorizationValue();
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }

            var isJweToken = _jwtParser.IsJweToken(token);
            var isJwsToken = _jwtParser.IsJwsToken(token);
            if (isJweToken && isJwsToken)
            {
                return null;
            }

            if (isJweToken)
            {
                token = await _jwtParser.Decrypt(token, cancellationToken);
            }

            return await _jwtParser.Unsign(token, cancellationToken);
        }

        private string ExtractAuthorizationValue()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return null;
            }

            var authorizationValue = Request.Headers["Authorization"].First();
            var splittedAuthorizationValue = authorizationValue.Split(' ');
            if (authorizationValue.StartsWith("Bearer") && splittedAuthorizationValue.Count() != 2)
            {
                return null;
            }

            return splittedAuthorizationValue.Last();
        }
    }
}
