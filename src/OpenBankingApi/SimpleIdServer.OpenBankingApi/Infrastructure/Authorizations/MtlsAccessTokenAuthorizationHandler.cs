// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt.Jws;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenBankingApi.Infrastructure.Authorizations
{
    public class MtlsAccessTokenAuthorizationHandler : AuthorizationHandler<MtlsAccessTokenRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJwsGenerator _jwsGenerator;
        private readonly OpenBankingApiOptions _options;
        private readonly ILogger<MtlsAccessTokenAuthorizationHandler> _logger;

        public MtlsAccessTokenAuthorizationHandler(
            IHttpContextAccessor  httpContextAccessor,
            IJwsGenerator jwsGenerator,
            IOptions<OpenBankingApiOptions> options, 
            ILogger<MtlsAccessTokenAuthorizationHandler> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _jwsGenerator = jwsGenerator;
            _options = options.Value;
            _logger = logger;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, MtlsAccessTokenRequirement requirement)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var result = await Authenticate(_httpContextAccessor.HttpContext);
            if (!result.Succeeded)
            {
                context.Fail();
                return;
            }

            if (request.Headers.TryGetValue("Authorization", out StringValues values))
            {
                var accessToken = ExtractAccessToken(values.First());
                var jws = _jwsGenerator.ExtractPayload(accessToken);
                if (!jws.ContainsKey(Jwt.Constants.OAuthClaims.Cnf))
                {
                    context.Fail();
                    return;
                }

                var cnf = jws[Jwt.Constants.OAuthClaims.Cnf] as JObject;
                if (cnf == null)
                {
                    context.Fail();
                    return;
                }

                var x5t = cnf[Jwt.Constants.OAuthClaims.X5TS256];
                if (x5t == null || string.IsNullOrWhiteSpace(x5t.ToString()))
                {
                    context.Fail();
                    return;
                }

                var clientCertificate = await _httpContextAccessor.HttpContext.Connection.GetClientCertificateAsync();
                var thumbprint = Hash(clientCertificate.RawData);
                if (thumbprint != x5t.ToString())
                {
                    context.Fail();
                    return;
                }

                context.Succeed(requirement);
                return;
            }

            context.Fail();
            return;
        }

        private async Task<AuthenticateResult> Authenticate(HttpContext context)
        {
            var x509AuthResult = await context.AuthenticateAsync(_options.CertificateAuthenticationScheme);
            if (!x509AuthResult.Succeeded)
            {
                _logger.LogError($"MTLS authentication failed : {x509AuthResult.Failure?.Message}");
                await context.ForbidAsync(_options.CertificateAuthenticationScheme);
            }

            return x509AuthResult;
        }

        private static string ExtractAccessToken(string value)
        {
            var splitted = value.Split(' ');
            if (splitted.Count() != 2 || !splitted[0].StartsWith("Bearer"))
            {
                return null;
            }

            return splitted.Last();
        }

        private static string Hash(byte[] payload)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashed = sha256.ComputeHash(payload);
                return Base64UrlTextEncoder.Encode(hashed);
            }
        }
    }
}
