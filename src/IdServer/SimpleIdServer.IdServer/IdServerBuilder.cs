// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.DataProtection;
using SimpleIdServer.Configuration;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Api.Realms;
using SimpleIdServer.IdServer.Config;
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
    private readonly ISidRoutesStore _sidRoutesStore;
    private readonly AuthenticationBuilder _authBuilder;
    private readonly FormBuilderRegistration _formBuilder;
    private readonly IDataProtectionBuilder _dataProtectionBuilder;
    private readonly IMvcBuilder _mvcBuilder;
    private readonly AutomaticConfigurationOptions _automaticConfigurationOptions;
    private readonly SidAuthCookie _sidAuthCookie;

    public IdServerBuilder(IServiceCollection serviceCollection, AuthenticationBuilder authBuilder, FormBuilderRegistration formBuidler, IDataProtectionBuilder dataProtectionBuilder, IMvcBuilder mvcBuilder, AutomaticConfigurationOptions automaticConfigurationOptions, SidAuthCookie sidAuthCookie)
    {
        _serviceCollection = serviceCollection;
        _authBuilder = authBuilder;
        _formBuilder = formBuidler;
        _dataProtectionBuilder = dataProtectionBuilder;
        _mvcBuilder = mvcBuilder;
        _automaticConfigurationOptions = automaticConfigurationOptions;
        _sidAuthCookie = sidAuthCookie;
        _sidRoutesStore = new SidRoutesStore();
        _serviceCollection.AddSingleton(_sidRoutesStore);

    }

    internal IServiceCollection Services => _serviceCollection;

    internal FormBuilderRegistration FormBuilder => _formBuilder;

    internal IDataProtectionBuilder DataProtectionBuilder => _dataProtectionBuilder;

    internal IMvcBuilder MvcBuilder => _mvcBuilder;

    internal AutomaticConfigurationOptions AutomaticConfigurationOptions => _automaticConfigurationOptions;

    internal SidAuthCookie SidAuthCookie => _sidAuthCookie;

    /// <summary>
    /// Adds a developer signing credential by registering a DefaultFileSerializedKeyStore with the standard keys.
    /// This is intended for development environments only.
    /// </summary>
    public IdServerBuilder AddDeveloperSigningCredential()
    {
        var keys = new List<SerializedFileKey>();
        keys.AddRange(SimpleIdServer.IdServer.Constants.StandardKeys);
        keys.Add(KeyGenerator.GenerateX509SigningCredentials(SimpleIdServer.IdServer.Constants.StandardRealms.Master, "certificate"));
        Services.AddSingleton<IFileSerializedKeyStore>(new DefaultFileSerializedKeyStore(keys));
        return this;
    }

    /// <summary>
    /// Adds an in-memory client repository using the provided list of clients.
    /// </summary>
    public IdServerBuilder AddInMemoryClients(List<Client> clients)
    {
        Services.AddSingleton<IClientRepository>(new DefaultClientRepository(clients));
        return this;
    }

    /// <summary>
    /// Adds an in-memory scope repository with the provided list of scopes.
    /// </summary>
    public IdServerBuilder AddInMemoryScopes(List<Scope> scopes)
    {
        Services.AddSingleton<IScopeRepository>(new DefaultScopeRepository(scopes));
        return this;
    }

    /// <summary>
    /// Adds an in-memory user repository using the provided list of users.
    /// </summary>
    public IdServerBuilder AddInMemoryUsers(List<User> users)
    {
        Services.AddSingleton<IUserRepository>(new DefaultUserRepository(users));
        return this;
    }

    /// <summary>
    /// Adds an in-memory language repository using the provided list of languages.
    /// </summary>
    public IdServerBuilder AddInMemoryLanguages(List<SimpleIdServer.IdServer.Domains.Language> languages)
    {
        Services.AddSingleton<ILanguageRepository>(new DefaultLanguageRepository(languages));
        return this;
    }

    /// <summary>
    /// Registers in-memory authentication scheme providers and their definitions.
    /// </summary>
    public IdServerBuilder AddInMemoryAuthenticationSchemes(List<SimpleIdServer.IdServer.Domains.AuthenticationSchemeProvider> authenticationSchemeProviders, List<AuthenticationSchemeProviderDefinition> authSchemeProviderDefinitions)
    {
        Services.AddSingleton<IAuthenticationSchemeProviderRepository>(new DefaultAuthenticationSchemeProviderRepository(authenticationSchemeProviders));
        Services.AddSingleton<IAuthenticationSchemeProviderDefinitionRepository>(new DefaultAuthenticationSchemeProviderDefinitionRepository(authSchemeProviderDefinitions));
        foreach(var def in authSchemeProviderDefinitions)
        {
            _automaticConfigurationOptions.Add(Type.GetType(def.HandlerFullQualifiedName));
        }

        return this;
    }

    /// <summary>
    /// Adds an in-memory realm repository using the provided list of realms.
    /// </summary>
    public IdServerBuilder AddInMemoryRealms(List<Realm> realms)
    {
        Services.AddSingleton<IRealmRepository>(new DefaultRealmRepository(realms));
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
    /// Enable realm.
    /// </summary>
    /// <returns></returns>
    public IdServerBuilder EnableRealm()
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

    internal void AddRoute(string routeName, string relativePattern, object def)
    {
        _sidRoutesStore.Add(new SidRoute
        {
            Default = def,
            Name = routeName,
            RelativePattern = relativePattern
        });
    }

    internal void Commit()
    {
        // TODO : Add the logic to update the record.
    }
}
