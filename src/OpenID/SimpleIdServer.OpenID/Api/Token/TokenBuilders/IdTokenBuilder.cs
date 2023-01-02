// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.Domains;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Token.TokenBuilders;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.OpenID.ClaimsEnrichers;
using SimpleIdServer.OpenID.DTOs;
using SimpleIdServer.OpenID.Helpers;
using SimpleIdServer.OpenID.SubjectTypeBuilders;
using SimpleIdServer.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using static SimpleIdServer.OpenID.SIDOpenIdConstants;

namespace SimpleIdServer.OpenID.Api.Token.TokenBuilders
{
    public class IdTokenBuilder : ITokenBuilder
    {
        private readonly IJwtBuilder _jwtBuilder;
        private readonly IEnumerable<IClaimsSource> _claimsSources;
        private readonly IEnumerable<ISubjectTypeBuilder> _subjectTypeBuilders;
        private readonly IAmrHelper _amrHelper;
        private readonly IUserRepository _userRepository;
        private readonly IClaimsJwsPayloadEnricher _claimsJwsPayloadEnricher;

        public IdTokenBuilder(
            IJwtBuilder jwtBuilder, 
            IEnumerable<IClaimsSource> claimsSources, 
            IEnumerable<ISubjectTypeBuilder> subjectTypeBuilders, 
            IAmrHelper amrHelper,
            IUserRepository userRepository, 
            IClaimsJwsPayloadEnricher claimsJwsPayloadEnricher)
        {
            _jwtBuilder = jwtBuilder;
            _claimsSources = claimsSources;
            _subjectTypeBuilders = subjectTypeBuilders;
            _amrHelper = amrHelper;
            _userRepository = userRepository;
            _claimsJwsPayloadEnricher = claimsJwsPayloadEnricher;
        }

        public string Name => TokenResponseParameters.IdToken;

        public virtual Task Build(IEnumerable<string> scopes, HandlerContext context, CancellationToken cancellationToken) =>  Build(scopes, new Dictionary<string, object>(), context, cancellationToken);

        public async Task Build(IEnumerable<string> scopes, Dictionary<string, object> claims, HandlerContext context, CancellationToken cancellationToken)
        {
            if (!scopes.Contains(StandardScopes.OpenIdScope.Name) || context.User == null)
                return;

            var openidClient = context.Client;
            var payload = await BuildIdToken(context, context.Request.RequestData, scopes, cancellationToken);
            var idToken = await _jwtBuilder.BuildClientToken(context.Client, payload, openidClient.IdTokenSignedResponseAlg, openidClient.IdTokenEncryptedResponseAlg, openidClient.IdTokenEncryptedResponseEnc, cancellationToken);
            context.Response.Add(Name, idToken);
        }

        public async Task Refresh(JsonObject previousQueryParameters, HandlerContext handlerContext, CancellationToken token)
        {
            if (!previousQueryParameters.ContainsKey(JwtRegisteredClaimNames.Sub))
                return;

            var scopes = previousQueryParameters.GetScopes();
            handlerContext.SetUser(await _userRepository.Query().FirstOrDefaultAsync(u => u.Id == previousQueryParameters[JwtRegisteredClaimNames.Sub].ToString(), token));
            var openidClient = handlerContext.Client;
            var payload = await BuildIdToken(handlerContext, previousQueryParameters, scopes, token);
            var idToken = await _jwtBuilder.BuildClientToken(handlerContext.Client, payload, openidClient.IdTokenSignedResponseAlg, openidClient.IdTokenEncryptedResponseAlg, openidClient.IdTokenEncryptedResponseEnc, token);
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
            if (currentContext.Response.TryGet(OAuth.DTOs.AuthorizationResponseParameters.AccessToken, out accessToken))
                claims.Add(JwtRegisteredClaimNames.AtHash, ComputeHash(accessToken));

            if (currentContext.Response.TryGet(OAuth.DTOs.AuthorizationResponseParameters.Code, out code))
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
                scopes = openidClient.Scopes.Where(s => requestedScopes.Contains(s.Name)).Select(s => new Scope { Name = s.Name });

            if (activeSession != null)
                claims.Add(JwtRegisteredClaimNames.Sid, activeSession.SessionId);

            EnrichWithScopeParameter(result, scopes, currentContext.User, subject);
            _claimsJwsPayloadEnricher.EnrichWithClaimsParameter(result, requestedClaims, currentContext.User, activeSession?.AuthenticationDateTime);
            foreach (var claimsSource in _claimsSources)
                await claimsSource.Enrich(result, openidClient, cancellationToken);

            var result = new SecurityTokenDescriptor
            {
                Issuer = currentContext.Request.IssuerName,
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddSeconds(openidClient.TokenExpirationTimeInSeconds ?? _options.DefaultTokenExpirationTimeInSeconds).ConvertToUnixTimestamp(),
                Claims = claims
            };
            return result;
        }

        public static void EnrichWithScopeParameter(JsonObject payload, IEnumerable<Scope> scopes, User user, string subject)
        {
            if (scopes != null)
            {
                foreach (var scope in scopes)
                {
                    foreach (var scopeClaim in scope.Claims)
                    {
                        if (scopeClaim.ClaimName == JwtRegisteredClaimNames.Sub)
                        {
                            payload.Add(JwtRegisteredClaimNames.Sub, subject);
                        }
                        else
                        {
                            var userClaims = user.Claims.Where(c => c.Type == scopeClaim.ClaimName);
                            payload.AddOrReplace(userClaims);
                        }
                    }
                }
            }
        }

        private static string ComputeHash(string str)
        {
            using (var sha256 = SHA256Managed.Create())
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
