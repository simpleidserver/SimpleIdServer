// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Options;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Middlewares
{
    public class MtlsAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<MtlsAuthenticationMiddleware> _logger;
        private readonly IdServerHostOptions _options;

        public MtlsAuthenticationMiddleware(RequestDelegate next, ILogger<MtlsAuthenticationMiddleware> logger, IOptions<IdServerHostOptions> options)
        {
            _next = next;
            _logger = logger;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (_options.MtlsEnabled)
            {
                if (context.Request.Path.ToString().Contains($"/{Constants.EndPoints.MtlsPrefix}"))
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
            var cert = await context.Connection.GetClientCertificateAsync();
            var x509AuthResult = await context.AuthenticateAsync(Constants.DefaultCertificateAuthenticationScheme);
            if (!x509AuthResult.Succeeded)
            {
                _logger.LogError($"MTLS authentication failed : {x509AuthResult.Failure?.Message}");
                var error = new JsonObject
                {
                    { ErrorResponseParameters.Error, ErrorCodes.INVALID_REQUEST },
                    { ErrorResponseParameters.ErrorDescription, ErrorMessages.CERTIFICATE_IS_REQUIRED }
                };
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.Headers.Add("Content-Types", "application/json");
                await context.Response.WriteAsync(error.ToString(), Encoding.UTF8);
            }

            return x509AuthResult;
        }
    }
}
