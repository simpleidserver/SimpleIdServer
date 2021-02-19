// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.OpenBankingApi.Host.Acceptance.Tests
{
    public class CertificateConfiguration : IStartupFilter
    {
        public CertificateConfiguration(IDictionary<string, X509Certificate2> certs) => Certificates = certs;

        public IDictionary<string, X509Certificate2> Certificates { get; }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                builder.Use((ctx, nxt) =>
                {
                    if (ctx.Request.Headers.TryGetValue("X-Testing-ClientCert", out var key) && Certificates.TryGetValue(key, out var clientCert))
                    {
                        ctx.Connection.ClientCertificate = clientCert;
                    }
                    return nxt();
                });
                next(builder);
            };
        }
    }
}
