// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace SimpleIdServer.Saml.Host.Acceptance.Tests
{
    public class CustomWebApplicationFactory<T> : WebApplicationFactory<T> where T : class
    {
        private readonly Action<IServiceCollection> _configureTestServices;

        public CustomWebApplicationFactory(Action<IServiceCollection> configureTestServices = null)
        {
            _configureTestServices = configureTestServices;
        }

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder()
                .UseStartup(typeof(T));
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.UseContentRoot(".");
            builder.ConfigureServices(collection =>
            {
                _configureTestServices?.Invoke(collection);
            });
        }
    }
}
