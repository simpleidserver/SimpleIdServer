// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using FormBuilder;
using Hangfire;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using SimpleIdServer.Configuration;
using SimpleIdServer.IdServer.Api.Realms;
using SimpleIdServer.IdServer.Config;
using SimpleIdServer.IdServer.Consumers;
using SimpleIdServer.IdServer.DataSeeder;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Jobs;
using SimpleIdServer.IdServer.Migrations.Static;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Provisioning;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.Stores.Default;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Extensions.DependencyInjection;

public class IdServerBuilder
{
    private readonly WebApplicationBuilder _builder;
    private readonly IServiceCollection _serviceCollection;
    private readonly ISidRoutesStore _sidRoutesStore;
    private readonly AuthenticationBuilder _authBuilder;
    private readonly FormBuilderRegistration _formBuilder;
    private readonly IDataProtectionBuilder _dataProtectionBuilder;
    private readonly IMvcBuilder _mvcBuilder;
    private readonly AutomaticConfigurationOptions _automaticConfigurationOptions;
    private readonly SidAuthCookie _sidAuthCookie;
    private readonly SidHangfire _sidHangfire;

    public IdServerBuilder(WebApplicationBuilder builder, IServiceCollection serviceCollection, AuthenticationBuilder authBuilder, FormBuilderRegistration formBuidler, IDataProtectionBuilder dataProtectionBuilder, IMvcBuilder mvcBuilder, AutomaticConfigurationOptions automaticConfigurationOptions, SidAuthCookie sidAuthCookie, SidHangfire sidHangfire)
    {
        _builder = builder;
        _serviceCollection = serviceCollection;
        _authBuilder = authBuilder;
        _formBuilder = formBuidler;
        _dataProtectionBuilder = dataProtectionBuilder;
        _mvcBuilder = mvcBuilder;
        _automaticConfigurationOptions = automaticConfigurationOptions;
        _sidAuthCookie = sidAuthCookie;
        _sidHangfire = sidHangfire;
        _sidRoutesStore = new SidRoutesStore();
        _serviceCollection.AddSingleton(_sidRoutesStore);
    }

    internal WebApplicationBuilder Builder => _builder;

    internal IServiceCollection Services => _serviceCollection;

    internal FormBuilderRegistration FormBuilder => _formBuilder;

    internal IDataProtectionBuilder DataProtectionBuilder => _dataProtectionBuilder;

    internal IMvcBuilder MvcBuilder => _mvcBuilder;

    internal AutomaticConfigurationOptions AutomaticConfigurationOptions => _automaticConfigurationOptions;

    internal SidAuthCookie SidAuthCookie => _sidAuthCookie;

    /// <summary>
    /// Configures the FormBuilder by applying the provided callback function.
    /// This allows customization of the FormBuilderRegistration instance.
    /// </summary>
    public IdServerBuilder ConfigureFormBuilder(Action<FormBuilderRegistration> cb)
    {
        cb(_formBuilder);
        return this;
    }


    /// <summary>
    /// Ignores server certificate validation errors. 
    /// WARNING: Use only for development purposes.
    /// </summary>
    public IdServerBuilder IgnoreCertificateError()
    {
        ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
        return this;
    }

    /// <summary>
    /// Adds a developer signing credential by registering a DefaultFileSerializedKeyStore with the standard keys.
    /// This is intended for development environments only.
    /// </summary>
    public IdServerBuilder AddDeveloperSigningCredential()
    {
        RemoveDataseeder<InitSerializedFileKeyDataSeeder>();
        var keys = new List<SerializedFileKey>();
        keys.AddRange(DefaultKeys.All);
        Services.AddSingleton<IFileSerializedKeyStore>(new DefaultFileSerializedKeyStore(keys));
        return this;
    }

    /// <summary>
    /// Adds an in-memory client repository using the provided list of clients.
    /// </summary>
    public IdServerBuilder AddInMemoryClients(List<Client> clients)
    {
        Services.AddSingleton(new StaticClientsDataSeeder(clients));
        Services.AddSingleton<IDataSeeder, InitStaticClientsDataSeeder>();
        return this;
    }

    /// <summary>
    /// Adds an in-memory scope repository with the provided list of scopes.
    /// </summary>
    public IdServerBuilder AddInMemoryScopes(List<Scope> scopes)
    {
        Services.AddSingleton(new StaticScopesDataSeeder(scopes));
        Services.AddSingleton<IDataSeeder, InitStaticScopesDataSeeder>();
        return this;
    }

    /// <summary>
    /// Adds an in-memory user repository using the provided list of users.
    /// </summary>
    public IdServerBuilder AddInMemoryUsers(List<User> users)
    {
        Services.AddSingleton(new StaticUsersDataSeeder(users));
        Services.AddTransient<IDataSeeder, InitStaticUsersDataSeeder>();
        return this;
    }

    /// <summary>
    /// Adds an in-memory language repository using the provided list of languages.
    /// </summary>
    public IdServerBuilder AddInMemoryLanguages(List<SimpleIdServer.IdServer.Domains.Language> languages)
    {
        Services.AddSingleton(new StaticLanguagesDataSeeder(languages));
        Services.AddSingleton<IDataSeeder, InitLanguageDataSeeder>();
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

    /// <summary>
    /// When FAPI2.0 is enabled then TLS connections shall be set up to use TLS version 1.2 or later.
    /// </summary>
    /// <param name="ssl"></param>
    /// <returns></returns>
    public IdServerBuilder EnableFapiSecurityProfile(SslProtocols ssl = SslProtocols.Tls12, ClientCertificateMode clientCertificate = ClientCertificateMode.AllowCertificate, Action<CertificateAuthenticationOptions> callback = null)
    {
        Action<HttpsConnectionAdapterOptions> cb = (o) =>
        {
            o.SslProtocols = ssl;
        };
        Services.Configure<KestrelServerOptions>(options =>
        {
            options.ConfigureHttpsDefaults((o) =>
            {
                o.SslProtocols = ssl;
                o.ClientCertificateMode = clientCertificate;
                cb(o);
            });
        });
        Services.AddCertificateForwarding(options =>
        {
            options.CertificateHeader = "ssl-client-cert";
            options.HeaderConverter = (headerValue) =>
            {
                X509Certificate2? clientCertificate = null;

                if (!string.IsNullOrWhiteSpace(headerValue))
                {
                    clientCertificate = X509Certificate2.CreateFromPem(WebUtility.UrlDecode(headerValue));
                }

                return clientCertificate!;
            };
        });
        Services.Configure<IdServerHostOptions>(o =>
        {
            o.MtlsEnabled = true;
        });
        _authBuilder.AddCertificate(SimpleIdServer.IdServer.Constants.DefaultCertificateAuthenticationScheme, callback != null ? callback : o => 
        {
            o.AllowedCertificateTypes = CertificateTypes.All;
        });
        return this;
    }

    /// <summary>
    /// Configures HTTP header forwarding for client IP and protocol information.
    /// </summary>
    public IdServerBuilder ForwardHttpHeader()
    {
        Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });
        return this;
    }

    // Enables the Back-Channel Authentication (CIBA) functionality.
    public IdServerBuilder EnableCiba()
    {
        _serviceCollection.AddTransient<BCNotificationJob>();
        _serviceCollection.Configure<IdServerHostOptions>(o =>
        {
            o.IsBCEnabled = true;
        });
        return this;
    }

    /// <summary>
    /// Configure hangfire.
    /// </summary>
    /// <param name="cb"></param>
    /// <returns></returns>
    public IdServerBuilder ConfigureHangfire(Action<IGlobalConfiguration> cb)
    {
        _sidHangfire.Callback = cb;
        return this;
    }

    /// <summary>
    /// Configure the key value storage.
    /// </summary>
    /// <returns></returns>
    public IdServerBuilder ConfigureKeyValueStore(Action<AutomaticConfigurationOptions> cb)
    {
        cb(_automaticConfigurationOptions);
        return this;
    }

    /// <summary>
    /// Use in memory implementation of mass transit.
    /// </summary>
    /// <returns></returns>
    public IdServerBuilder EnableMasstransit(Action<IBusRegistrationConfigurator> cb, Action migrationServiceCb = null)
    {
        if(_serviceCollection.Any(s => s.ServiceType == typeof(IBus)))
        {
            throw new InvalidOperationException("MassTransit is already configured by the AddSidIdentityServer operation. To disable this configuration, set the skipMassTransitRegistration parameter to true.");
        }

        if(migrationServiceCb != null)
        {
            migrationServiceCb();
        }

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

    /// <summary>
    /// Disables the sharing of the authentication cookie across different applications.
    /// </summary>
    /// <returns></returns>
    public IdServerBuilder DisableSharingAuthCookie()
    {
        _sidAuthCookie.CookieSameSiteMode = SameSiteMode.Lax;
        _sidAuthCookie.CookieSecurePolicy = CookieSecurePolicy.SameAsRequest;
        return this;
    }

    /// <summary>
    /// Enables the in-memory MassTransit transport, configuring the publish message scheduler and endpoints.
    /// </summary>
    internal IdServerBuilder EnableInMemoryMasstransit()
    {
        return EnableMasstransit(cb =>
        {
            cb.UsingInMemory((ctx, cfg) =>
            {
                cfg.UsePublishMessageScheduler();
                cfg.ConfigureEndpoints(ctx);
            });
        });
    }

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

    #region Other

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
    /// Seeds administration data including groups and users initialization.
    /// </summary>
    public IdServerBuilder SeedAdministrationData(List<string> redirectUrls, List<string> postLogoutUrls, string backchannelLogoutUrl, List<Scope> additionalScopes)
    {
        _serviceCollection.AddTransient<IDataSeeder, InitAdministrativeScopeDataSeeder>();
        _serviceCollection.AddTransient<IDataSeeder, InitGroupDataSeeder>();
        _serviceCollection.AddTransient<IDataSeeder, InitUserDataSeeder>();
        _serviceCollection.AddTransient<IDataSeeder>((s) =>
        {
            var scope = s.CreateScope();
            var transactionBuidler = scope.ServiceProvider.GetRequiredService<ITransactionBuilder>();
            var realmRepository = scope.ServiceProvider.GetRequiredService<IRealmRepository>();
            var scopeRepository = scope.ServiceProvider.GetRequiredService<IScopeRepository>();
            var clientRepository = scope.ServiceProvider.GetRequiredService<IClientRepository>();
            var dataseederRepository = scope.ServiceProvider.GetRequiredService<IDataSeederExecutionHistoryRepository>();
            return new InitAdministrativeClientDataseeder(redirectUrls, postLogoutUrls, backchannelLogoutUrl, additionalScopes, transactionBuidler, realmRepository, scopeRepository, clientRepository, dataseederRepository);
        });
        _serviceCollection.AddTransient<IDataSeeder, AssignTemplateScopeToClientDataSeeder>();
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

    private void RemoveDataseeder<T>() where T : IDataSeeder
    {
        var service = _serviceCollection.SingleOrDefault(s => s.ImplementationType == typeof(T));
        if(service != null)
        {
            _serviceCollection.Remove(service);
        }
    }
}
