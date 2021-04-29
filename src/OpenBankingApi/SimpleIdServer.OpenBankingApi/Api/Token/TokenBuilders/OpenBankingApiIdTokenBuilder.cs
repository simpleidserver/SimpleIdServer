// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt.Extensions;
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.Jwt.Jws.Handlers;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenID.Api.Token.TokenBuilders;
using SimpleIdServer.OpenID.ClaimsEnrichers;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.Helpers;
using SimpleIdServer.OpenID.SubjectTypeBuilders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenBankingApi.Api.Token.TokenBuilders
{
    public class OpenBankingApiIdTokenBuilder : IdTokenBuilder
    {
        private readonly IOpenBankingApiAuthRequestEnricher _openBankingApiAuthRequestEnricher;
        private readonly IJwtBuilder _jwtBuilder;
        private readonly OpenBankingApiOptions _options;
        private Dictionary<IEnumerable<string>, Func<byte[], byte[]>> MAPPING_HASH_CALLBACK = new Dictionary<IEnumerable<string>, Func<byte[], byte[]>>
        {
            { new [] { ECDSAP256SignHandler.ALG_NAME, PS256SignHandler.ALG_NAME, RSA256SignHandler.ALG_NAME }, Hash256 },
            { new [] { ECDSAP384SignHandler.ALG_NAME, PS384SignHandler.ALG_NAME, RSA384SignHandler.ALG_NAME }, Hash384 },
            { new [] { ECDSAP512SignHandler.ALG_NAME, PS512SignHandler.ALG_NAME, RSA512SignHandler.ALG_NAME }, Hash512 },
        };

        public OpenBankingApiIdTokenBuilder(
            IOpenBankingApiAuthRequestEnricher openBankingApiAuthRequestEnricher,
            IJwtBuilder jwtBuilder, 
            IOptions<OpenBankingApiOptions> options,
            IEnumerable<IClaimsSource> claimsSources, 
            IEnumerable<ISubjectTypeBuilder> subjectTypeBuilders, 
            IAmrHelper amrHelper, 
            IOAuthUserQueryRepository oauthUserQueryRepository, 
            IClaimsJwsPayloadEnricher claimsJwsPayloadEnricher) : base(jwtBuilder, claimsSources, subjectTypeBuilders, amrHelper, oauthUserQueryRepository, claimsJwsPayloadEnricher)
        {
            _openBankingApiAuthRequestEnricher = openBankingApiAuthRequestEnricher;
            _jwtBuilder = jwtBuilder;
            _options = options.Value;
        }

        public override async Task Build(IEnumerable<string> scopes, HandlerContext context, CancellationToken cancellationToken)
        {
            if (context.User == null)
            {
                return;
            }

            var openidClient = (OpenIdClient)context.Client;
            var payload = await BuildIdToken(context, context.Request.RequestData, scopes, cancellationToken);
            var idToken = await _jwtBuilder.BuildClientToken(context.Client, payload, openidClient.IdTokenSignedResponseAlg, openidClient.IdTokenEncryptedResponseAlg, openidClient.IdTokenEncryptedResponseEnc);
            context.Response.Add(Name, idToken);
        }

        protected override async Task<JwsPayload> BuildIdToken(HandlerContext currentContext, JObject queryParameters, IEnumerable<string> requestedScopes, CancellationToken cancellationToken)
        {
            var result = await base.BuildIdToken(currentContext, queryParameters, requestedScopes, cancellationToken);
            var state = queryParameters.GetStateFromAuthorizationRequest();
            if (!string.IsNullOrWhiteSpace(state))
            {
                var openidClient = (OpenIdClient)currentContext.Client;
                var hash = MAPPING_HASH_CALLBACK.First(k => k.Key.Contains(openidClient.IdTokenSignedResponseAlg)).Value(ASCIIEncoding.ASCII.GetBytes(state));
                var half = hash.Take((hash.Length / 2)).ToArray();
                result.Add(Constants.OpenBankingApiClaimNames.SHash, half.Base64EncodeBytes());
            }

            await _openBankingApiAuthRequestEnricher.Enrich(result, queryParameters, cancellationToken);
            if (result.ContainsKey(_options.OpenBankingApiConsentClaimName))
            {
                if (result.ContainsKey(Jwt.Constants.UserClaims.Subject))
                {
                    result[Jwt.Constants.UserClaims.Subject] = result[_options.OpenBankingApiConsentClaimName];
                }
                else
                {
                    result.Add(Jwt.Constants.UserClaims.Subject, result[_options.OpenBankingApiConsentClaimName]);
                }
            }

            return result;
        }

        private static byte[] Hash256(byte[] state)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(state);
            }
        }

        private static byte[] Hash384(byte[] state)
        {
            using (var sha384 = SHA384.Create())
            {
                return sha384.ComputeHash(state);
            }
        }

        private static byte[] Hash512(byte[] state)
        {
            using (var sha512 = SHA512.Create())
            {
                return sha512.ComputeHash(state);
            }
        }
    }
}
