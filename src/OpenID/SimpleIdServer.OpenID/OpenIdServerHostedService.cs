// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID
{
    public class OpenIdServerHostedService : IHostedService
    {
        private readonly IOpenIdJobServer _openIdJobServer;

        public OpenIdServerHostedService(IServiceScopeFactory serviceScopeFactory)
        {
            var scope = serviceScopeFactory.CreateScope();
            _openIdJobServer = scope.ServiceProvider.GetRequiredService<IOpenIdJobServer>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _openIdJobServer.Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _openIdJobServer.Stop();
            return Task.CompletedTask;
        }
    }
}
