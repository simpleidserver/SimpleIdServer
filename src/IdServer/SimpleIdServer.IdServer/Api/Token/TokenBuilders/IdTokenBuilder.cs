// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.ClaimsEnricher;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.SubjectTypeBuilders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using static SimpleIdServer.IdServer.Constants;

namespace SimpleIdServer.IdServer.Api.Token.TokenBuilders
{
    public class IdTokenBuilder : ITokenBuilder
    {
        private readonly IdServerHostOptions _options;
        private readonly IJwtBuilder _jwtBuilder;
        private readonly IClaimsEnricher _claimsEnricher;
        private readonly IEnumerable<ISubjectTypeBuilder> _subjectTypeBuilders;
        private readonly IAmrHelper _amrHelper;
        private readonly IClaimsJwsPayloadEnricher _claimsJwsPayloadEnricher;

        public IdTokenBuilder(
            IOptions<IdServerHostOptions> options,
            IJwtBuilder jwtBuilder,
            IClaimsEnricher claimsEnricher,
            IEnumerable<ISubjectTypeBuilder> subjectTypeBuilders,
            IAmrHelper amrHelper,
            IClaimsJwsPayloadEnricher claimsJwsPayloadEnricher)
        {
            _options = options.Value;
            _jwtBuilder = jwtBuilder;
            _claimsEnricher = claimsEnricher;
            _subjectTypeBuilders = subjectTypeBuilders;
            _amrHelper = amrHelper;
            _claimsJwsPayloadEnricher = claimsJwsPayloadEnricher;
        }

        public string Name => TokenResponseParameters.IdToken;

        public virtual Task Build(IEnumerable<string> scopes, HandlerContext context, CancellationToken cancellationToken) => Build(scopes, new Dictionary<string, object>(), context, cancellationToken);

        public async Task Build(IEnumerable<string> scopes, Dictionary<string, object> claims, HandlerContext context, CancellationToken cancellationToken)
        {
            if (!scopes.Contains(StandardScopes.OpenIdScope.Name) || context.User == null)
                return;

            var openidClient = context.Client;
            var payload = await BuildIdToken(context, context.Request.RequestData, scopes, cancellationToken);
            var idToken = await _jwtBuilder.BuildClientToken(context.Client, payload, (openidClient.IdTokenSignedResponseAlg ?? _options.DefaultTokenSignedResponseAlg), openidClient.IdTokenEncryptedResponseAlg, openidClient.IdTokenEncryptedResponseEnc, cancellationToken);
            context.Response.Add(Name, idToken);
        }

        public async Task Refresh(JsonObject previousQueryParameters, HandlerContext handlerContext, CancellationToken token)
        {
            if (!previousQueryParameters.ContainsKey(JwtRegisteredClaimNames.Sub))
                return;

            var scopes = previousQueryParameters.GetScopes();
            var openidClient = handlerContext.Client;
            var payload = await BuildIdToken(handlerContext, previousQueryParameters, scopes, token);
            var idToken = await _jwtBuilder.BuildClientToken(handlerContext.Client, payload, (openidClient.IdTokenSignedResponseAlg ?? _options.DefaultTokenSignedResponseAlg), openidClient.IdTokenEncryptedResponseAlg, openidClient.IdTokenEncryptedResponseEnc, token);
            handlerContext.Response.Add(Name, idToken);
        }

        protected virtual async Task<SecurityTokenDescriptor> BuildIdToken(HandlerContext currentContext, JsonObject queryParameters, IEnumerable<string> requestedScopes, CancellationToken cancellationToken)
        {
            var openidClient = currentContext.Client;
            var claims = new Dictionary<string, object>
            {
                { System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Aud,  new[] { openidClient.ClientId, currentContext.Request.IssuerName } },
                { JwtRegisteredClaimNames.Azp, openidClient.ClientId }
            };

            var maxAge = queryParameters.GetMaxAgeFromAuthorizationRequest();
            var nonce = queryParameters.GetNonceFromAuthorizationRequest();
            var acrValues = queryParameters.GetAcrValuesFromAuthorizationRequest();
            var requestedClaims = queryParameters.GetClaimsFromAuthorizationRequest();
            var subjectTypeBuilder = _subjectTypeBuilders.First(f => f.SubjectType == (string.IsNullOrWhiteSpace(openidClient.SubjectType) ? PublicSubjectTypeBuilder.SUBJECT_TYPE : openidClient.SubjectType));
            var subject = await subjectTypeBuilder.Build(currentContext, cancellationToken);
            string accessToken, code;
            if (currentContext.Response.TryGet(AuthorizationResponseParameters.AccessToken, out accessToken))
                claims.Add(JwtRegisteredClaimNames.AtHash, ComputeHash(accessToken));

            if (currentContext.Response.TryGet(AuthorizationResponseParameters.Code, out code))
                claims.Add(JwtRegisteredClaimNames.CHash, ComputeHash(code));

            var activeSession = currentContext.User.ActiveSession;
            if (maxAge != null && activeSession != null)
                claims.Add(JwtRegisteredClaimNames.AuthTime, activeSession.AuthenticationDateTime.ConvertToUnixTimestamp());

            if (!string.IsNullOrWhiteSpace(nonce))
                claims.Add(JwtRegisteredClaimNames.Nonce, nonce);

            var defaultAcr = await _amrHelper.FetchDefaultAcr(acrValues, requestedClaims, openidClient, cancellationToken);
            if (defaultAcr != null)
            {
                claims.Add(JwtRegisteredClaimNames.Amr, defaultAcr.AuthenticationMethodReferences);
                claims.Add(JwtRegisteredClaimNames.Acr, defaultAcr.Name);
            }

            IEnumerable<Scope> scopes = new Scope[1] { StandardScopes.OpenIdScope };
            var responseTypes = queryParameters.GetResponseTypesFromAuthorizationRequest();
            if (responseTypes.Count() == 1 && responseTypes.First() == AuthorizationResponseParameters.IdToken)
                scopes = openidClient.Scopes.Where(s => requestedScopes.Contains(s.Name));

            if (activeSession != null)
                claims.Add(JwtRegisteredClaimNames.Sid, activeSession.SessionId);

            EnrichWithScopeParameter(claims, scopes, currentContext.User, subject);
            _claimsJwsPayloadEnricher.EnrichWithClaimsParameter(claims, requestedClaims, currentContext.User, activeSession?.AuthenticationDateTime);
            await _claimsEnricher.Enrich(currentContext.User, claims, openidClient, cancellationToken);

            var result = new SecurityTokenDescriptor
            {
                Issuer = currentContext.Request.IssuerName,
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddSeconds(openidClient.TokenExpirationTimeInSeconds ?? _options.DefaultTokenExpirationTimeInSeconds),
                Claims = claims
            };
            return result;
        }

        public static void EnrichWithScopeParameter(Dictionary<string, object> claims, IEnumerable<Scope> scopes, User user, string subject)
        {
            if (scopes != null)
            {
                foreach (var scope in scopes)
                {
                    foreach (var scopeClaim in scope.Claims)
                    {
                        if (scopeClaim.ClaimName == JwtRegisteredClaimNames.Sub)
                            claims.Add(JwtRegisteredClaimNames.Sub, subject);
                        else
                        {
                            var userClaims = user.Claims.Where(c => c.Type == scopeClaim.ClaimName);
                            foreach (var userClaim in userClaims)
                            {
                                if (claims.ContainsKey(userClaim.Type)) claims[userClaim.Type] = userClaim.Value;
                                else claims.Add(userClaim.Type, userClaim.Value);
                            }
                        }
                    }
                }
            }
        }

        private static string ComputeHash(string str)
        {
            using (var sha256 = SHA256.Create())
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
