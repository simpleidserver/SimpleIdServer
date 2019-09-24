// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt.Extensions;
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Token.TokenBuilders;
using SimpleIdServer.OAuth.Domains.Scopes;
using SimpleIdServer.OAuth.Domains.Users;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.OpenID.ClaimsEnrichers;
using SimpleIdServer.OpenID.DTOs;
using SimpleIdServer.OpenID.Extensions;
using SimpleIdServer.OpenID.Helpers;
using SimpleIdServer.OpenID.SubjectTypeBuilders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static SimpleIdServer.Jwt.Constants;

namespace SimpleIdServer.OpenID.Api.Token.TokenBuilders
{
    public class IdTokenBuilder : ITokenBuilder
    {
        private readonly IJwtBuilder _jwtBuilder;
        private readonly IEnumerable<IClaimsSource> _claimsSources;
        private readonly IEnumerable<ISubjectTypeBuilder> _subjectTypeBuilders;
        private readonly IAmrHelper _amrHelper;

        public IdTokenBuilder(IJwtBuilder jwtBuilder, IEnumerable<IClaimsSource> claimsSources,
            IEnumerable<ISubjectTypeBuilder> subjectTypeBuilders, IAmrHelper amrHelper)
        {
            _jwtBuilder = jwtBuilder;
            _claimsSources = claimsSources;
            _subjectTypeBuilders = subjectTypeBuilders;
            _amrHelper = amrHelper;
        }

        public string Name => TokenResponseParameters.IdToken;

        public async Task Build(IEnumerable<string> scopes, HandlerContext context, Dictionary<string, object> claims = null)
        {
            if (!scopes.Contains("openid") || context.User == null)
            {
                return;
            }

            var payload = await BuildIdToken(context).ConfigureAwait(false);
            var idToken = await _jwtBuilder.BuildIdentityToken(context.Client, payload);
            context.Response.Add(Name, idToken);
        }

        public Task Build(JwsPayload jwsPayload, HandlerContext handlerContext)
        {
            return Task.FromResult(0);
        }

        private async Task<JwsPayload> BuildIdToken(HandlerContext context)
        {
            var result = new JwsPayload
            {
                { OAuthClaims.Audiences, new [] { context.Client.ClientId, context.Request.IssuerName } },
                { OAuthClaims.Issuer, context.Request.IssuerName },
                { OAuthClaims.Iat, DateTime.UtcNow.ConvertToUnixTimestamp() },
                { OAuthClaims.ExpirationTime, DateTime.UtcNow.AddSeconds(context.Client.TokenExpirationTimeInSeconds).ConvertToUnixTimestamp() },
                { OAuthClaims.Azp, context.Client.ClientId }
            };
            var subjectTypeBuilder = _subjectTypeBuilders.First(f => f.SubjectType == (string.IsNullOrWhiteSpace(context.Client.SubjectType) ? PublicSubjectTypeBuilder.SUBJECT_TYPE : context.Client.SubjectType));
            var subject = await subjectTypeBuilder.Build(context);
            result.Add(UserClaims.Subject, subject);
            var maxAge = context.Request.QueryParameters.GetMaxAgeFromAuthorizationRequest();
            var nonce = context.Request.QueryParameters.GetNonceFromAuthorizationRequest();
            var acrValues = context.Request.QueryParameters.GetAcrValuesFromAuthorizationRequest();
            var requestedScopes = context.Request.QueryParameters.GetScopesFromAuthorizationRequest();
            var requestedClaims = context.Request.QueryParameters.GetClaimsFromAuthorizationRequest();
            if (maxAge != null)
            {
                result.Add(OAuthClaims.AuthenticationTime, context.Request.AuthDateTime.Value.ConvertToUnixTimestamp());
            }

            if (!string.IsNullOrWhiteSpace(nonce))
            {
                result.Add(OAuthClaims.Nonce, nonce);
            }

            string accessToken, code;
            if (context.Response.TryGet(OAuth.DTOs.AuthorizationResponseParameters.AccessToken, out accessToken))
            {
                result.Add(OAuthClaims.AtHash, ComputeHash(accessToken));
            }

            if (context.Response.TryGet(OAuth.DTOs.AuthorizationResponseParameters.Code, out code))
            {
                result.Add(OAuthClaims.CHash, ComputeHash(code));
            }

            var acr = await _amrHelper.FetchDefaultAcr(acrValues, context.Client);
            if (acr != null)
            {
                result.Add(OAuthClaims.Amr, acr.AuthenticationMethodReferences);
                result.Add(OAuthClaims.Acr, acr.Name);
            }

            var scopes = context.Client.AllowedScopes.Where(s => requestedScopes.Any(r => r == s.Name));
            EnrichWithScopeParameter(result, scopes, context.User);
            EnrichWithClaimsParameter(result, requestedClaims, context.User, context.Request.AuthDateTime.Value);
            foreach(var claimsSource in _claimsSources)
            {
                await claimsSource.Enrich(result, context.Client).ConfigureAwait(false);
            }

            return result;
        }

        public static void EnrichWithScopeParameter(JwsPayload payload, IEnumerable<OAuthScope> scopes, OAuthUser user)
        {
            if (scopes != null)
            {
                foreach (var scope in scopes)
                {
                    foreach (var scopeClaim in scope.Claims)
                    {
                        if (user.Claims.ContainsKey(scopeClaim))
                        {
                            payload.TryAdd(scopeClaim, user.Claims.First(c => c.Key == scopeClaim).Value);
                        }
                    }
                }
            }
        }

        public static void EnrichWithClaimsParameter(JwsPayload payload, IEnumerable<AuthorizationRequestClaimParameter> requestedClaims, OAuthUser user, DateTime? authDateTime, AuthorizationRequestClaimTypes claimType = AuthorizationRequestClaimTypes.IdToken)
        {
            if (requestedClaims != null)
            {
                foreach (var claim in requestedClaims.Where(c => c.Type == claimType))
                {
                    if (USER_CLAIMS.Contains(claim.Name))
                    {
                        payload.TryAdd(claim.Name, user.Claims.First(c => c.Key == claim.Name).Value);
                    }
                    else
                    {
                        if (claim.Name == OAuthClaims.AuthenticationTime && authDateTime != null)
                        {
                            payload.Add(OAuthClaims.AuthenticationTime, authDateTime.Value.ConvertToUnixTimestamp());
                        }
                    }
                }
            }
        }

        private static string ComputeHash(string str)
        {
            using (var sha256 = new SHA256Managed())
            {
                var bytes = Encoding.Default.GetBytes(str);
                var hash = sha256.ComputeHash(bytes);
                var payload = new byte[16];
                Array.Copy(hash, payload, 16);
                return payload.Base64EncodeBytes();
            }
        }
    }
}
