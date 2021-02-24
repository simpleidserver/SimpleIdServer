// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt.Extensions;
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Token.TokenBuilders;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenID.ClaimsEnrichers;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.DTOs;
using SimpleIdServer.OpenID.Extensions;
using SimpleIdServer.OpenID.Helpers;
using SimpleIdServer.OpenID.SubjectTypeBuilders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
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
        private readonly IOAuthUserQueryRepository _oauthUserRepository;
        private readonly IClaimsJwsPayloadEnricher _claimsJwsPayloadEnricher;

        public IdTokenBuilder(IJwtBuilder jwtBuilder, IEnumerable<IClaimsSource> claimsSources, IEnumerable<ISubjectTypeBuilder> subjectTypeBuilders, IAmrHelper amrHelper, IOAuthUserQueryRepository oauthUserQueryRepository, IClaimsJwsPayloadEnricher claimsJwsPayloadEnricher)
        {
            _jwtBuilder = jwtBuilder;
            _claimsSources = claimsSources;
            _subjectTypeBuilders = subjectTypeBuilders;
            _amrHelper = amrHelper;
            _oauthUserRepository = oauthUserQueryRepository;
            _claimsJwsPayloadEnricher = claimsJwsPayloadEnricher;
        }

        public string Name => TokenResponseParameters.IdToken;

        public virtual async Task Build(IEnumerable<string> scopes, HandlerContext context, CancellationToken cancellationToken)
        {
            if (!scopes.Contains(SIDOpenIdConstants.StandardScopes.OpenIdScope.Name) || context.User == null)
            {
                return;
            }

            var openidClient = (OpenIdClient)context.Client;
            var payload = await BuildIdToken(context, context.Request.Data, scopes, cancellationToken);
            var idToken = await _jwtBuilder.BuildClientToken(context.Client, payload, openidClient.IdTokenSignedResponseAlg, openidClient.IdTokenEncryptedResponseAlg, openidClient.IdTokenEncryptedResponseEnc);
            context.Response.Add(Name, idToken);
        }

        public async Task Refresh(JObject previousQueryParameters, HandlerContext currentContext, CancellationToken token)
        {
            if (!previousQueryParameters.ContainsKey(UserClaims.Subject))
            {
                return;
            }

            currentContext.SetUser(await _oauthUserRepository.FindOAuthUserByLogin(previousQueryParameters[UserClaims.Subject].ToString(), token));
            var openidClient = (OpenIdClient)currentContext.Client;
            var payload = await BuildIdToken(currentContext, previousQueryParameters, new string[0], token);
            var idToken = await _jwtBuilder.BuildClientToken(currentContext.Client, payload, openidClient.IdTokenSignedResponseAlg, openidClient.IdTokenEncryptedResponseAlg, openidClient.IdTokenEncryptedResponseEnc);
            currentContext.Response.Add(Name, idToken);
        }

        protected virtual async Task<JwsPayload> BuildIdToken(HandlerContext currentContext, JObject queryParameters, IEnumerable<string> requestedScopes, CancellationToken cancellationToken)
        {
            var openidClient = (OpenIdClient)currentContext.Client;
            var result = new JwsPayload
            {
                { OAuthClaims.Audiences, new [] { openidClient.ClientId, currentContext.Request.IssuerName } },
                { OAuthClaims.Issuer, currentContext.Request.IssuerName },
                { OAuthClaims.Iat, DateTime.UtcNow.ConvertToUnixTimestamp() },
                { OAuthClaims.ExpirationTime, DateTime.UtcNow.AddSeconds(openidClient.TokenExpirationTimeInSeconds).ConvertToUnixTimestamp() },
                { OAuthClaims.Azp, openidClient.ClientId }
            };
            var maxAge = queryParameters.GetMaxAgeFromAuthorizationRequest();
            var nonce = queryParameters.GetNonceFromAuthorizationRequest();
            var acrValues = queryParameters.GetAcrValuesFromAuthorizationRequest();
            var requestedClaims = queryParameters.GetClaimsFromAuthorizationRequest();
            var subjectTypeBuilder = _subjectTypeBuilders.First(f => f.SubjectType == (string.IsNullOrWhiteSpace(openidClient.SubjectType) ? PublicSubjectTypeBuilder.SUBJECT_TYPE : openidClient.SubjectType));
            var subject = await subjectTypeBuilder.Build(currentContext);
            string accessToken, code;
            if (currentContext.Response.TryGet(OAuth.DTOs.AuthorizationResponseParameters.AccessToken, out accessToken))
            {
                result.Add(OAuthClaims.AtHash, ComputeHash(accessToken));
            }

            if (currentContext.Response.TryGet(OAuth.DTOs.AuthorizationResponseParameters.Code, out code))
            {
                result.Add(OAuthClaims.CHash, ComputeHash(code));
            }

            if (maxAge != null)
            {
                result.Add(OAuthClaims.AuthenticationTime, currentContext.User.AuthenticationTime.Value.ConvertToUnixTimestamp());
            }

            if (!string.IsNullOrWhiteSpace(nonce))
            {
                result.Add(OAuthClaims.Nonce, nonce);
            }

            var defaultAcr = await _amrHelper.FetchDefaultAcr(acrValues, requestedClaims, openidClient, cancellationToken);
            if (defaultAcr != null)
            {
                result.Add(OAuthClaims.Amr, defaultAcr.AuthenticationMethodReferences);
                result.Add(OAuthClaims.Acr, defaultAcr.Name);
            }

            var scopes = openidClient.AllowedOpenIdScopes.Where(s => requestedScopes.Any(r => r == s.Name));
            EnrichWithScopeParameter(result, scopes, currentContext.User, subject);
            _claimsJwsPayloadEnricher.EnrichWithClaimsParameter(result, requestedClaims, currentContext.User, currentContext.User.AuthenticationTime);
            foreach (var claimsSource in _claimsSources)
            {
                await claimsSource.Enrich(result, openidClient).ConfigureAwait(false);
            }

            return result;
        }

        public static void EnrichWithScopeParameter(JwsPayload payload, IEnumerable<OpenIdScope> scopes, OAuthUser user, string subject)
        {
            if (scopes != null)
            {
                foreach (var scope in scopes)
                {
                    foreach (var scopeClaim in scope.Claims)
                    {
                        if (scopeClaim.ClaimName == UserClaims.Subject)
                        {
                            payload.Add(UserClaims.Subject, subject);
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
