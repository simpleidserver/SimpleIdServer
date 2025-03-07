// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Hangfire;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Api.Realms;
using SimpleIdServer.IdServer.Consumers;
using SimpleIdServer.IdServer.Jobs;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Provisioning;
using SimpleIdServer.IdServer.UI.AuthProviders;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public class IdServerBuilder
    {
        private readonly IServiceCollection _serviceCollection;
        private readonly AuthenticationBuilder _authBuilder;

        public IdServerBuilder(IServiceCollection serviceCollection, AuthenticationBuilder authBuilder)
        {
            _serviceCollection = serviceCollection;
            _authBuilder = authBuilder;
        }

        public IServiceCollection Services => _serviceCollection;

        #region Authentication & Authorization

        public IdServerBuilder EnableConfigurableAuthentication()
        {
            _serviceCollection.AddSingleton<IAuthenticationSchemeProvider, DynamicAuthenticationSchemeProvider>();
            _serviceCollection.AddSingleton<ISIDAuthenticationSchemeProvider>(x => x.GetService<IAuthenticationSchemeProvider>() as ISIDAuthenticationSchemeProvider);
            _serviceCollection.AddScoped<IAuthenticationHandlerProvider, DynamicAuthenticationHandlerProvider>();
            return this;
        }

        public IdServerBuilder AddAuthentication(Action<AuthBuilder> callback = null)
        {
            _authBuilder
                .AddCookie(Constants.DefaultExternalCookieAuthenticationScheme);
            if(callback != null)
                callback(new AuthBuilder(_serviceCollection, _authBuilder));

            return this;
        }

        #endregion

        #region CIBA

        /// <summary>
        /// Add back channel authentication (CIBA).
        /// </summary>
        /// <returns></returns>
        public IdServerBuilder AddBackChannelAuthentication()
        {
            _serviceCollection.AddTransient<BCNotificationJob>();
            _serviceCollection.AddHangfireServer();
            _serviceCollection.Configure<IdServerHostOptions>(o =>
            {
                o.IsBCEnabled = true;
            });
            return this;
        }

        #endregion

        #region Other

        /// <summary>
        /// IdentityServer can be hosted in several Realm.
        /// </summary>
        /// <returns></returns>
        public IdServerBuilder UseRealm()
        {
            _serviceCollection.Configure<IdServerHostOptions>(o =>
            {
                o.UseRealm = true;
            });
            return this;
        }

        /// <summary>
        /// Authorization server accepts authorization request data only via PAR.
        /// </summary>
        /// <returns></returns>
        public IdServerBuilder PARIsRequired()
        {
            _serviceCollection.Configure<IdServerHostOptions>(o =>
            {
                o.RequiredPushedAuthorizationRequest = true;
            });
            return this;
        }

        public IdServerBuilder UseInMemoryMassTransit()
        {
            return this.UseMassTransit(o =>
            {
                o.UsingInMemory((ctx, cfg) =>
                {
                    cfg.UsePublishMessageScheduler();
                    cfg.ConfigureEndpoints(ctx);
                });
            });
        }

        /// <summary>
        /// Use in memory implementation of mass transit.
        /// </summary>
        /// <returns></returns>
        public IdServerBuilder UseMassTransit(Action<IBusRegistrationConfigurator> cb)
        {
            _serviceCollection.AddMassTransitTestHarness((o) =>
            {
                o.AddPublishMessageScheduler();
                o.AddHangfireConsumers();
                o.AddConsumer<ExtractUsersFaultConsumer>();
                o.AddConsumer<ImportUsersFaultConsumer>();
                o.AddConsumer<IdServerEventsConsumer>();
                o.AddConsumer<ExtractUsersConsumer, ExtractUsersConsumerDefinition>();
                o.AddConsumer<ImportUsersConsumer, ImportUsersConsumerDefinition>();
                o.AddConsumer<RemoveRealmCommandConsumer, RemoveRealmConsumerDefinition>();
                cb(o);
            });
            return this;
        }

        #endregion
    }
}
