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
        private readonly IAmrHelper _amrHelper;
        private readonly IClaimsJwsPayloadEnricher _claimsJwsPayloadEnricher;
        private readonly IClaimsExtractor _claimsExtractor;

        public IdTokenBuilder(
            IOptions<IdServerHostOptions> options,
            IJwtBuilder jwtBuilder,
            IClaimsEnricher claimsEnricher,
            IAmrHelper amrHelper,
            IClaimsJwsPayloadEnricher claimsJwsPayloadEnricher,
            IClaimsExtractor claimsExtractor)
        {
            _options = options.Value;
            _jwtBuilder = jwtBuilder;
            _claimsEnricher = claimsEnricher;
            _amrHelper = amrHelper;
            _claimsJwsPayloadEnricher = claimsJwsPayloadEnricher;
            _claimsExtractor = claimsExtractor;
        }

        public string Name => TokenResponseParameters.IdToken;

        public virtual async Task Build(BuildTokenParameter parameter, HandlerContext context, CancellationToken cancellationToken, bool useOriginalRequest = false)
        {
            if (!parameter.Scopes.Contains(StandardScopes.OpenIdScope.Name) || context.User == null)
                return;

            var openidClient = context.Client;
            var payload = await BuildIdToken(context, useOriginalRequest ? context.OriginalRequest : context.Request.RequestData, parameter.Scopes, parameter.Claims, cancellationToken);
            var idToken = await _jwtBuilder.BuildClientToken(context.Realm, context.Client, payload, (openidClient.IdTokenSignedResponseAlg ?? _options.DefaultTokenSignedResponseAlg), openidClient.IdTokenEncryptedResponseAlg, openidClient.IdTokenEncryptedResponseEnc, cancellationToken);
            context.Response.Add(Name, idToken);
        }

        protected virtual async Task<SecurityTokenDescriptor> BuildIdToken(HandlerContext currentContext, JsonObject queryParameters, IEnumerable<string> requestedScopes, IEnumerable<AuthorizedClaim> requestedClaims, CancellationToken cancellationToken)
        {
            var openidClient = currentContext.Client;
            var claims = new Dictionary<string, object>
            {
                { System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Aud,  new[] { openidClient.ClientId, currentContext.GetIssuer() } },
                { JwtRegisteredClaimNames.Azp, openidClient.ClientId }
            };

            var maxAge = queryParameters.GetMaxAgeFromAuthorizationRequest();
            var nonce = queryParameters.GetNonceFromAuthorizationRequest();
            var acrValues = queryParameters.GetAcrValuesFromAuthorizationRequest();
            string accessToken, code;
            if (currentContext.Response.TryGet(AuthorizationResponseParameters.AccessToken, out accessToken))
                claims.Add(JwtRegisteredClaimNames.AtHash, ComputeHash(accessToken));

            if (currentContext.Response.TryGet(AuthorizationResponseParameters.Code, out code))
                claims.Add(JwtRegisteredClaimNames.CHash, ComputeHash(code));

            var activeSession = currentContext.User.GetActiveSession(currentContext.Realm ?? Constants.DefaultRealm);
            if (maxAge != null && activeSession != null)
                claims.Add(JwtRegisteredClaimNames.AuthTime, activeSession.AuthenticationDateTime.ConvertToUnixTimestamp());

            if (!string.IsNullOrWhiteSpace(nonce))
                claims.Add(JwtRegisteredClaimNames.Nonce, nonce);

            var defaultAcr = await _amrHelper.FetchDefaultAcr(currentContext.Realm, acrValues, requestedClaims, openidClient, cancellationToken);
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

            var claimsDic = await _claimsExtractor.ExtractClaims(new ClaimsExtractionParameter
            {
                Protocol = ScopeProtocols.OPENID,
                Context = currentContext,
                Scopes = scopes
            });
            foreach (var claim in claimsDic)
                claims.Add(claim.Key, claim.Value);

            _claimsJwsPayloadEnricher.EnrichWithClaimsParameter(claims, requestedClaims, currentContext.User, activeSession?.AuthenticationDateTime);
            await _claimsEnricher.Enrich(currentContext.User, claims, openidClient, cancellationToken);

            var result = new SecurityTokenDescriptor
            {
                Issuer = currentContext.GetIssuer(),
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddSeconds(openidClient.TokenExpirationTimeInSeconds ?? _options.DefaultTokenExpirationTimeInSeconds),
                Claims = claims
            };
            return result;
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
