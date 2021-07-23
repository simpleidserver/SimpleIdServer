// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SimpleIdServer.Saml.Exceptions;
using SimpleIdServer.Saml.Extensions;
using System.Threading.Tasks;

namespace SimpleIdServer.Saml.Middlewares
{
    public class SamlExceptionMiddleware
    {
        private readonly ILogger<SamlExceptionMiddleware> _logger;
        private readonly RequestDelegate _next;

        public SamlExceptionMiddleware(
            ILogger<SamlExceptionMiddleware> logger,
            RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(SamlException ex)
            {
                _logger.LogError(ex.ToString());
                var xml = ex.BuildResponse().SerializeToXmlElement().OuterXml;
                context.Response.Clear();
                context.Response.StatusCode = (int)ex.HttpStatusCode;
                context.Response.ContentType = "application/xml";
                await context.Response.WriteAsync(xml);
            }
        }
    }
}
