// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.DataProtection;
using SimpleIdServer.IdServer.Api.Realms;
using SimpleIdServer.IdServer.Consumers;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Jobs;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Provisioning;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.Stores.Default;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection;

public class IdServerBuilder
{
    private readonly IServiceCollection _serviceCollection;
    private readonly AuthenticationBuilder _authBuilder;
    private readonly FormBuilderRegistration _formBuilder;
    private readonly IDataProtectionBuilder _dataProtectionBuilder;

    public IdServerBuilder(IServiceCollection serviceCollection, AuthenticationBuilder authBuilder, FormBuilderRegistration formBuidler, IDataProtectionBuilder dataProtectionBuilder)
    {
        _serviceCollection = serviceCollection;
        _authBuilder = authBuilder;
        _formBuilder = formBuidler;
        _dataProtectionBuilder = dataProtectionBuilder;
    }

    internal IServiceCollection Services => _serviceCollection;

    internal FormBuilderRegistration FormBuilder => _formBuilder;

    internal IDataProtectionBuilder DataProtectionBuilder => _dataProtectionBuilder;

    public IdServerBuilder AddDeveloperSigningCredential()
    {
        Services.AddSingleton<IFileSerializedKeyStore>(new DefaultFileSerializedKeyStore(SimpleIdServer.IdServer.Constants.StandardKeys));
        return this;
    }

    public IdServerBuilder AddInMemoryClients(List<Client> clients)
    {
        Services.AddSingleton<IClientRepository>(new DefaultClientRepository(clients));
        return this;
    }

    public IdServerBuilder AddInMemoryScopes(List<Scope> scopes)
    {
        Services.AddSingleton<IScopeRepository>(new DefaultScopeRepository(scopes));
        return this;
    }

    #region Authentication & Authorization

    public IdServerBuilder AddMutualAuthentication(Action<CertificateAuthenticationOptions> callback = null)
    {
        Services.Configure<IdServerHostOptions>(o =>
        {
            o.MtlsEnabled = true;
        });
        _authBuilder.AddCertificate(SimpleIdServer.IdServer.Constants.DefaultCertificateAuthenticationScheme, callback != null ? callback : o => { });
        return this;
    }

    public IdServerBuilder AddMutualAuthenticationSelfSigned()
    {
        Services.Configure<IdServerHostOptions>(o =>
        {
            o.MtlsEnabled = true;
        });
        _authBuilder.AddCertificate(SimpleIdServer.IdServer.Constants.DefaultCertificateAuthenticationScheme, o =>
        {
            o.AllowedCertificateTypes = CertificateTypes.SelfSigned;
        });
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
    public IdServerBuilder PushAuthorizationRequestIsRequired()
    {
        _serviceCollection.Configure<IdServerHostOptions>(o =>
        {
            o.RequiredPushedAuthorizationRequest = true;
        });
        return this;
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
