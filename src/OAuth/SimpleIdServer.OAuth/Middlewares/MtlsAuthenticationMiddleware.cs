// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.OAuth.Options;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Middlewares
{
    public class MtlsAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<MtlsAuthenticationMiddleware> _logger;
        private readonly OAuthHostOptions _options;

        public MtlsAuthenticationMiddleware(
            RequestDelegate next, 
            ILogger<MtlsAuthenticationMiddleware> logger,
            IOptions<OAuthHostOptions> options)
        {
            _next = next;
            _logger = logger;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (_options.MtlsEnabled)
            {
                if (context.Request.Path.StartsWithSegments($"/{Constants.EndPoints.MtlsPrefix}"))
                {
                    var result = await Authenticate(context);
                    if (!result.Succeeded)
                    {
                        return;
                    }
                }
            }

            await _next.Invoke(context);
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
    }
}
