// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using FormBuilder;
using Hangfire;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using SimpleIdServer.Configuration;
using SimpleIdServer.Did.Key;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Api.Authorization;
using SimpleIdServer.IdServer.Api.Authorization.ResponseModes;
using SimpleIdServer.IdServer.Api.Authorization.ResponseTypes;
using SimpleIdServer.IdServer.Api.Authorization.Validators;
using SimpleIdServer.IdServer.Api.BCAuthorize;
using SimpleIdServer.IdServer.Api.Configuration;
using SimpleIdServer.IdServer.Api.DeviceAuthorization;
using SimpleIdServer.IdServer.Api.Jwks;
using SimpleIdServer.IdServer.Api.OpenIdConfiguration;
using SimpleIdServer.IdServer.Api.Register;
using SimpleIdServer.IdServer.Api.Token;
using SimpleIdServer.IdServer.Api.Token.Handlers;
using SimpleIdServer.IdServer.Api.Token.Helpers;
using SimpleIdServer.IdServer.Api.Token.PKCECodeChallengeMethods;
using SimpleIdServer.IdServer.Api.Token.TokenBuilders;
using SimpleIdServer.IdServer.Api.Token.TokenProfiles;
using SimpleIdServer.IdServer.Api.Token.Validators;
using SimpleIdServer.IdServer.Api.TokenIntrospection;
using SimpleIdServer.IdServer.Auth;
using SimpleIdServer.IdServer.Authenticate;
using SimpleIdServer.IdServer.Authenticate.AssertionParsers;
using SimpleIdServer.IdServer.Authenticate.Handlers;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.ClaimsEnricher;
using SimpleIdServer.IdServer.ClaimTokenFormats;
using SimpleIdServer.IdServer.Config;
using SimpleIdServer.IdServer.Console;
using SimpleIdServer.IdServer.Console.Services;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Extractors;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Helpers.Models;
using SimpleIdServer.IdServer.Infastructures;
using SimpleIdServer.IdServer.Jobs;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Migrations;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Seeding;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.Stores.Default;
using SimpleIdServer.IdServer.SubjectTypeBuilders;
using SimpleIdServer.IdServer.TokenTypes;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.UI.AuthProviders;
using SimpleIdServer.IdServer.UI.Infrastructures;
using SimpleIdServer.IdServer.UI.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Constants = SimpleIdServer.IdServer.Constants;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures and sets up the Identity Server with all required services and dependencies.
    /// </summary>
    /// <param name="app">The WebApplicationBuilder instance to configure services.</param>
    /// <param name="callback">Optional callback action to configure IdServerHostOptions.</param>
    /// <param name="skipMasstransitRegistration">If true, skips the automatic registration of in-memory Masstransit configuration.</param>
    /// <returns>An IdServerBuilder instance that allows further configuration of the Identity Server.</returns>
    public static IdServerBuilder AddSidIdentityServer
    (
        this WebApplicationBuilder app,
        Action<IdServerHostOptions>? callback = null,
        bool skipMasstransitRegistration = false
    )
    {
        var services = app.Services;
        if (callback != null) services.Configure(callback);
        else services.Configure<IdServerHostOptions>(o => { });
        services.Configure<RouteOptions>(opt =>
        {
            opt.ConstraintMap.Add("realmPrefix", typeof(RealmRoutePrefixConstraint));
        });
        Tracing.Init();
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        services.AddSingleton<ISidEndpointStore, SidEndpointStore>();
        ConfigureDataSeeders(services);
        var mvcBuilder = services.AddControllersWithViews();
        var dataProtectionBuilder = services.AddDataProtection();
        services.AddDataseeder();
        services.AddDistributedMemoryCache();
        services.AddDidKey();
        ConfigureBlazorAndLocalization(services);
        ConfigureIdentityServer(services);
        var formBuilder = ConfigureFormBuilder(services);
        var sidHangfire = new SidHangfire();
        ConfigureHangfire(services, sidHangfire);
        var autoConfig = ConfigureCentralizedConfiguration(app);
        ConfigureConsoleNotification(services);
        services.AddHttpContextAccessor();
        var sidAuthCookie = new SidAuthCookie();
        var authBuilder = ConfigureAuth(services, sidAuthCookie);
        var result = new IdServerBuilder(services, authBuilder, formBuilder, dataProtectionBuilder, mvcBuilder, autoConfig, sidAuthCookie, sidHangfire);
        if(!skipMasstransitRegistration)
        {
            result.EnableInMemoryMasstransit();
        }

        return result;
    }

    private static void ConfigureDataSeeders(IServiceCollection services)
    {
        services.AddTransient<IDataSeeder, InitRealmDataSeeder>();
        services.AddTransient<IDataSeeder, InitSerializedFileKeyDataSeeder>();
        services.AddTransient<IDataSeeder, InitCertificateAuthoritiesDataseeder>();
        services.AddTransient<IDataSeeder, InitLanguageDataSeeder>();
        services.AddTransient<IDataSeeder, InitScopeDataSeeder>();
    }

    private static void ConfigureIdentityServer(IServiceCollection services)
    {
        services.AddResponseModeHandlers()
            .AddOAuthClientAuthentication()
            .AddClientAssertionParsers()
            .AddOauthJwksApi()
            .AddOAuthTokenApi()
            .AddOAuthAuthorizationApi()
            .AddOAuthJwt()
            .AddLib()
            .AddConfigurationApi()
            .AddUI()
            .AddSubjectTypeBuilder()
            .AddClaimsEnricher()
            .AddOAuthIntrospectionTokenApi()
            .AddRegisterApi()
            .AddBCAuthorizeApi()
            .AddDeviceAuthorizationApi()
            .AddTokenTypes()
            .AddStores();
    }

    private static void ConfigureConsoleNotification(IServiceCollection services)
    {
        services.AddTransient<IUserAuthenticationService, UserConsoleAuthenticationService>();
        services.AddTransient<IUserConsoleAuthenticationService, UserConsoleAuthenticationService>();
        services.AddTransient<IResetPasswordService, UserConsolePasswordResetService>();
        services.AddTransient<IAuthenticationMethodService, ConsoleAuthenticationService>();
        services.AddTransient<IUserConsoleNotificationService, ConsoleNotificationService>();
        services.AddTransient<IUserNotificationService, ConsoleNotificationService>();
        services.AddTransient<IWorkflowLayoutService, ConsoleAuthWorkflowLayout>();
    }

    private static void ConfigureBlazorAndLocalization(IServiceCollection services)
    {
        services.AddServerSideBlazor().AddCircuitOptions(o =>
        {
            o.DetailedErrors = true;
        }).AddHubOptions(o =>
        {
            o.MaximumReceiveMessageSize = 102400000;
        });
        services.AddRazorPages().AddRazorRuntimeCompilation();
        services.AddLocalization();
    }

    private static FormBuilderRegistration ConfigureFormBuilder(IServiceCollection services)
    {
        const string cookieName = "XSFR-TOKEN";
        services.AddAntiforgery(c =>
        {
            c.Cookie.Name = cookieName;
        });
        var formBuilder = services.AddFormBuilder(cb =>
        {
            cb.AntiforgeryCookieName = cookieName;
        });
        return formBuilder;
    }

    private static void ConfigureHangfire(IServiceCollection services, SidHangfire sidHangfire)
    {
        services.AddHangfire(o => {
            o.UseRecommendedSerializerSettings();
            o.UseIgnoredAssemblyVersionTypeResolver();
            sidHangfire.Callback(o);
        });
        services.AddHangfireServer();
    }

    private static AutomaticConfigurationOptions ConfigureCentralizedConfiguration(WebApplicationBuilder app)
    {
        return app.AddAutomaticConfiguration(o =>
        {
            o.Add<UserLockingOptions>();
            o.Add<IdServerConsoleOptions>();
        });
    }

    private static AuthenticationBuilder ConfigureAuth(IServiceCollection services, SidAuthCookie sidAuthCookie)
    {
        services.AddSingleton<IAuthenticationSchemeProvider, DynamicAuthenticationSchemeProvider>();
        services.AddSingleton(x => x.GetService<IAuthenticationSchemeProvider>() as ISIDAuthenticationSchemeProvider);
        services.AddScoped<IAuthenticationHandlerProvider, DynamicAuthenticationHandlerProvider>();
        services.AddAuthorization();
        services.Configure<AuthorizationOptions>(o =>
        {
            o.AddPolicy(Constants.AuthenticatedPolicyName, p => p.RequireAuthenticatedUser());
        });
        var authBuilder = services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddIdServerCookie(CookieAuthenticationDefaults.AuthenticationScheme, null, opts =>
            {
                sidAuthCookie.Callback(opts);
                opts.Events.OnSigningIn += (CookieSigningInContext ctx) =>
                {
                    if (ctx.Principal != null && ctx.Principal.Identity != null && ctx.Principal.Identity.IsAuthenticated)
                    {
                        var nameIdentifier = ctx.Principal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                        var ticket = new AuthenticationTicket(ctx.Principal, ctx.Properties, ctx.Scheme.Name);
                        var realmStore = ctx.HttpContext.RequestServices.GetRequiredService<IRealmStore>();
                        var cookieValue = ctx.Options.TicketDataFormat.Protect(ticket, GetTlsTokenBinding(ctx));
                        ctx.Options.CookieManager.AppendResponseCookie(
                            ctx.HttpContext,
                            $"{IdServerCookieAuthenticationHandler.GetCookieName(realmStore.Realm, ctx.Options.Cookie.Name)}-{nameIdentifier.SanitizeNameIdentifier()}",
                            cookieValue,
                            ctx.CookieOptions);
                    }

                    return Task.CompletedTask;
                };
                opts.Events.OnSigningOut += (CookieSigningOutContext ctx) =>
                {
                    string nameIdentifier = null;
                    if (ctx.Properties != null && ctx.Properties.Items.ContainsKey(Constants.LogoutUserKey)) nameIdentifier = ctx.Properties.Items[Constants.LogoutUserKey];
                    if (string.IsNullOrWhiteSpace(nameIdentifier) && ctx.HttpContext.User != null && ctx.HttpContext.User.Identity != null && ctx.HttpContext.User.Identity.IsAuthenticated)
                    {
                        nameIdentifier = ctx.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                    }

                    if (!string.IsNullOrWhiteSpace(nameIdentifier))
                    {
                        var realmStore = ctx.HttpContext.RequestServices.GetRequiredService<IRealmStore>();
                        var realm = realmStore.Realm;
                        if (ctx.Properties != null && ctx.Properties.Items.ContainsKey(Constants.RealmKey)) realm = ctx.Properties.Items[Constants.RealmKey];
                        ctx.Options.CookieManager.DeleteCookie(
                                ctx.HttpContext,
                                $"{IdServerCookieAuthenticationHandler.GetCookieName(realm, ctx.Options.Cookie.Name)}-{nameIdentifier.SanitizeNameIdentifier()}",
                                ctx.CookieOptions);
                    }
                    return Task.CompletedTask;
                };
            })
            .AddCookie(SimpleIdServer.IdServer.Constants.DefaultExternalCookieAuthenticationScheme);
        return authBuilder;

        string GetTlsTokenBinding(CookieSigningInContext context)
        {
            var binding = context.HttpContext.Features.Get<ITlsTokenBindingFeature>()?.GetProvidedTokenBindingId();
            return binding == null ? null : Convert.ToBase64String(binding);
        }
    }

    /// <summary>
    /// Registrates the services to allow the seeding via JSON file.
    /// </summary>
    /// <param name="services">The services collection.</param>
    /// <param name="configuration">The application's configuration.</param>
    /// <returns>The services collection.</returns>
    public static IServiceCollection AddJsonSeeding(this IServiceCollection services, IConfiguration configuration)
    {
        string jsonSeedsFilePath = configuration.GetValue<string>(ConfigurationSections.JsonSeedsFilePath);
        services.AddOptionsWithValidateOnStart<JsonSeedingOptions>()
            .Configure(options =>
            {
                options.SeedFromJson = !string.IsNullOrEmpty(jsonSeedsFilePath);
                options.JsonFilePath = jsonSeedsFilePath;
            })
            .Validate(
                config => !config.SeedFromJson || (config.SeedFromJson && File.Exists(config.JsonFilePath)),
                "The JSON file for seeding must exists."
            );

        services.AddTransient<ISeedStrategy, JsonSeedStrategy>();
        return services;
    }

    public static IServiceCollection AddEntitySeeders(this IServiceCollection services, Type referenceType)
    {
        Type[] entitySeeders = referenceType.Assembly.GetTypes()
            .Where(t => t.IsClass && t.GetInterfaces().Any(i => i.Name.Contains("IEntitySeeder")))
            .ToArray();

        foreach (Type entitySeeder in entitySeeders)
        {
            Type interfaceType = entitySeeder.GetInterfaces().First(i => i.Name.Contains("IEntitySeeder"));
            services.TryAddTransient(interfaceType, entitySeeder);
        }

        return services;
    }

    #region Register identity server depedencies

    private static IServiceCollection AddResponseModeHandlers(this IServiceCollection services)
    {
        services.AddTransient<IOAuthResponseMode, QueryResponseModeHandler>();
        services.AddTransient<IOAuthResponseMode, FragmentResponseModeHandler>();
        services.AddTransient<IOAuthResponseMode, FormPostResponseModeHandler>();
        services.AddTransient<IOAuthResponseMode, FormPostJwtResponseModeHandler>();
        services.AddTransient<IOAuthResponseMode, QueryJwtResponseModeHandler>();
        services.AddTransient<IOAuthResponseMode, FragmentJwtResponseModeHandler>();
        services.AddTransient<IOAuthResponseMode, JwtResponseModeHandler>();
        services.AddTransient<IOAuthResponseModeHandler, QueryResponseModeHandler>();
        services.AddTransient<IOAuthResponseModeHandler, FragmentResponseModeHandler>();
        services.AddTransient<IOAuthResponseModeHandler, FormPostResponseModeHandler>();
        services.AddTransient<IOAuthResponseModeHandler, FormPostJwtResponseModeHandler>();
        services.AddTransient<IOAuthResponseModeHandler, QueryJwtResponseModeHandler>();
        services.AddTransient<IOAuthResponseModeHandler, FragmentJwtResponseModeHandler>();
        services.AddTransient<IOAuthResponseModeHandler, JwtResponseModeHandler>();
        services.AddTransient<IResponseModeHandler, ResponseModeHandler>();
        return services;
    }

    private static IServiceCollection AddOAuthClientAuthentication(this IServiceCollection services)
    {
        services.AddTransient<IAuthenticateClient>(s => new AuthenticateClient(s.GetService<IClientRepository>(), s.GetServices<IOAuthClientAuthenticationHandler>(), s.GetServices<IClientAssertionParser>(), s.GetService<IBusControl>(), s.GetService<IOptions<IdServerHostOptions>>()));
        services.AddTransient<IOAuthClientAuthenticationHandler, OAuthClientPrivateKeyJwtAuthenticationHandler>();
        services.AddTransient<IOAuthClientAuthenticationHandler, OAuthClientSecretBasicAuthenticationHandler>();
        services.AddTransient<IOAuthClientAuthenticationHandler, OAuthClientSecretJwtAuthenticationHandler>();
        services.AddTransient<IOAuthClientAuthenticationHandler, OAuthClientSecretPostAuthenticationHandler>();
        services.AddTransient<IOAuthClientAuthenticationHandler, OAuthClientTlsClientAuthenticationHandler>();
        services.AddTransient<IOAuthClientAuthenticationHandler, OAuthClientSelfSignedTlsClientAuthenticationHandler>();
        services.AddTransient<IPkceVerifier, PkceVerifier>();
        return services;
    }

    private static IServiceCollection AddClientAssertionParsers(this IServiceCollection services)
    {
        services.AddTransient<IClientAssertionParser, ClientJwtAssertionParser>();
        return services;
    }

    private static IServiceCollection AddOauthJwksApi(this IServiceCollection services)
    {
        services.AddTransient<IJwksRequestHandler, JwksRequestHandler>();
        services.AddTransient<IKeyStore, InMemoryKeyStore>();
        services.AddTransient<ICertificateAuthorityStore, CertificateAuthorityStore>();
        return services;
    }

    private static IServiceCollection AddDeviceAuthorizationApi(this IServiceCollection services)
    {
        services.AddTransient<IDeviceAuthorizationRequestHandler, DeviceAuthorizationRequestHandler>();
        services.AddTransient<IDeviceAuthorizationRequestValidator, DeviceAuthorizationRequestValidator>();
        return services;
    }

    private static IServiceCollection AddSubjectTypeBuilder(this IServiceCollection services)
    {
        services.AddTransient<ISubjectTypeBuilder, PairWiseSubjectTypeBuidler>();
        services.AddTransient<ISubjectTypeBuilder, PublicSubjectTypeBuilder>();
        return services;
    }

    private static IServiceCollection AddClaimsEnricher(this IServiceCollection services)
    {
        services.AddTransient<IRelayClaimsExtractor, HttpClaimsExtractor>();
        services.AddTransient<IClaimsEnricher, ClaimsEnricher>();
        return services;
    }

    private static IServiceCollection AddOAuthTokenApi(this IServiceCollection services)
    {
        services.AddTransient<ITokenRequestHandler>(s => new TokenRequestHandler(s.GetServices<IGrantTypeHandler>()));
        services.AddTransient<IClientCredentialsGrantTypeValidator, ClientCredentialsGrantTypeValidator>();
        services.AddTransient<IRefreshTokenGrantTypeValidator, RefreshTokenGrantTypeValidator>();
        services.AddTransient<IAuthorizationCodeGrantTypeValidator, AuthorizationCodeGrantTypeValidator>();
        services.AddTransient<IRevokeTokenValidator, RevokeTokenValidator>();
        services.AddTransient<IPasswordGrantTypeValidator, PasswordGrantTypeValidator>();
        services.AddTransient<IGrantedTokenHelper, GrantedTokenHelper>();
        services.AddTransient<IAcrHelper, AcrHelper>();
        services.AddTransient<ITokenExchangeValidator, TokenExchangeValidator>();
        services.AddTransient<IGrantTypeHandler, TokenExchangeHandler>();
        services.AddTransient<IGrantTypeHandler, ClientCredentialsHandler>();
        services.AddTransient<IGrantTypeHandler, RefreshTokenHandler>();
        services.AddTransient<IGrantTypeHandler, PasswordHandler>();
        services.AddTransient<IGrantTypeHandler, AuthorizationCodeHandler>();
        services.AddTransient<IGrantTypeHandler, CIBAHandler>();
        services.AddTransient<IGrantTypeHandler, UmaTicketHandler>();
        services.AddTransient<IGrantTypeHandler, PreAuthorizedCodeHandler>();
        services.AddTransient<IGrantTypeHandler, DeviceCodeHandler>();
        services.AddTransient<IGrantTypeHandler, TokenExchangePreAuthorizedCodeHandler>();
        services.AddTransient<ICIBAGrantTypeValidator, CIBAGrantTypeValidator>();
        services.AddTransient<IClientAuthenticationHelper, ClientAuthenticationHelper>();
        services.AddTransient<IRevokeTokenRequestHandler, RevokeTokenRequestHandler>();
        services.AddTransient<ITokenExchangePreAuthorizedCodeValidator, TokenExchangePreAuthorizedCodeValidator>();
        services.AddTransient<ITokenProfile, BearerTokenProfile>();
        services.AddTransient<ITokenProfile, MacTokenProfile>();
        services.AddTransient<ITokenBuilder, AccessTokenBuilder>();
        services.AddTransient<ITokenBuilder, IdTokenBuilder>();
        services.AddTransient<ITokenBuilder, RefreshTokenBuilder>();
        services.AddTransient<IClaimsJwsPayloadEnricher, ClaimsJwsPayloadEnricher>();
        services.AddTransient<ICodeChallengeMethodHandler, PlainCodeChallengeMethodHandler>();
        services.AddTransient<ICodeChallengeMethodHandler, S256CodeChallengeMethodHandler>();
        services.AddTransient<IClientHelper, StandardClientHelper>();
        services.AddTransient<IAmrHelper, AmrHelper>();
        services.AddTransient<IWorkflowHelper, WorkflowHelper>();
        services.AddTransient<IUmaPermissionTicketHelper, UMAPermissionTicketHelper>();
        services.AddTransient<IExtractRequestHelper, ExtractRequestHelper>();
        services.AddTransient<IGrantHelper, GrantHelper>();
        services.AddTransient<IClaimTokenFormat, OpenIDClaimTokenFormat>();
        services.AddTransient<IUmaTicketGrantTypeValidator, UmaTicketGrantTypeValidator>();
        services.AddTransient<IAuthenticationHelper, AuthenticationHelper>();
        services.AddTransient<IScopeClaimsExtractor, ScopeClaimsExtractor>();
        services.AddTransient<IClaimsExtractor, ClaimsExtractor>();
        services.AddTransient<IClaimExtractor, AttributeClaimExtractor>();
        services.AddTransient<IClaimExtractor, PropertyClaimExtractor>();
        services.AddTransient<IClaimExtractor, ScimClaimExtractor>();
        services.AddTransient<IClaimExtractor, SubClaimExtractor>();
        services.AddTransient<IUserHelper, UserHelper>();
        services.AddTransient<IPreAuthorizedCodeValidator, PreAuthorizedCodeValidator>();
        services.AddTransient<IKeyStore, InMemoryKeyStore>();
        services.AddTransient<IDeviceCodeGrantTypeValidator, DeviceCodeGrantTypeValidator>();
        services.AddTransient<IDPOPProofValidator, DPOPProofValidator>();
        services.AddTransient<ISessionHelper, SessionHelper>();
        services.AddTransient<UserSessionJob>();
        return services;
    }

    private static IServiceCollection AddOAuthIntrospectionTokenApi(this IServiceCollection services)
    {
        services.AddTransient<ITokenIntrospectionRequestHandler, TokenIntrospectionRequestHandler>();
        return services;
    }

    private static IServiceCollection AddOAuthAuthorizationApi(this IServiceCollection services)
    {
        services.AddTransient<IAuthorizationRequestHandler, AuthorizationRequestHandler>();
        services.AddTransient<IResponseTypeHandler, AuthorizationCodeResponseTypeHandler>();
        services.AddTransient<IResponseTypeHandler, IdTokenResponseTypeHandler>();
        services.AddTransient<IResponseTypeHandler, TokenResponseTypeHandler>();
        services.AddTransient<IAuthorizationRequestValidator, OAuthAuthorizationRequestValidator>();
        services.AddTransient<IAuthorizationRequestEnricher, AuthorizationRequestEnricher>();
        services.AddTransient<IUserConsentFetcher, OAuthUserConsentFetcher>();
        services.AddTransient<IAuthorizationCallbackRequestHandler, AuthorizationCallbackRequestHandler>();
        services.AddTransient<IAuthorizationCallbackRequestValidator, AuthorizationCallbackRequestValidator>();
        return services;
    }

    private static IServiceCollection AddBCAuthorizeApi(this IServiceCollection services)
    {
        services.AddTransient<IBCAuthorizeHandler, BCAuthorizeHandler>();
        services.AddTransient<IBCAuthorizeRequestValidator, BCAuthorizeRequestValidator>();
        services.AddTransient<IBCNotificationService, BCNotificationService>();
        return services;
    }

    private static IServiceCollection AddTokenTypes(this IServiceCollection services)
    {
        services.AddTransient<ITokenTypeService, AccessTokenTypeService>();
        services.AddTransient<ITokenTypeService, IdTokenTypeService>();
        return services;
    }

    private static IServiceCollection AddOAuthJwt(this IServiceCollection services)
    {
        services.AddTransient<IJwtBuilder, JwtBuilder>();
        return services;
    }

    private static IServiceCollection AddLib(this IServiceCollection services)
    {
        services.AddTransient<IHttpClientFactory, HttpClientFactory>();
        services.AddScoped<IRealmStore, CookieRealmStore>();
        return services;
    }

    private static IServiceCollection AddConfigurationApi(this IServiceCollection services)
    {
        services.AddTransient<IOAuthConfigurationRequestHandler, OAuthConfigurationRequestHandler>();
        services.AddTransient<IOpenidConfigurationRequestHandler, OpenidConfigurationRequestHandler>();
        services.AddTransient<IOAuthWorkflowConverter, OAuthWorkflowConverter>();
        return services;
    }

    private static IServiceCollection AddUI(this IServiceCollection services)
    {
        services.AddTransient<IOTPAuthenticator, HOTPAuthenticator>();
        services.AddTransient<IOTPAuthenticator, TOTPAuthenticator>();
        services.AddTransient<IOTPQRCodeGenerator, OTPQRCodeGenerator>();
        services.AddTransient<ISessionManager, SessionManager>();
        services.AddTransient<IUserTransformer, UserTransformer>();
        return services;
    }

    private static IServiceCollection AddStores(this IServiceCollection services)
    {
        services.AddSingleton<IApiResourceRepository>(new DefaultApiResourceRepository(new List<ApiResource>()));
        services.AddSingleton<IAuditEventRepository>(new DefaultAuditEventRepository(new List<AuditEvent>()));
        services.AddSingleton<IAuthenticationContextClassReferenceRepository>(new DefaultAuthenticationContextClassReferenceRepository(new List<AuthenticationContextClassReference>()));
        services.AddSingleton<IAuthenticationSchemeProviderDefinitionRepository>(new DefaultAuthenticationSchemeProviderDefinitionRepository(new List<AuthenticationSchemeProviderDefinition>()));
        services.AddSingleton<IAuthenticationSchemeProviderRepository>(new DefaultAuthenticationSchemeProviderRepository(new List<SimpleIdServer.IdServer.Domains.AuthenticationSchemeProvider>()));
        services.AddSingleton<IBCAuthorizeRepository>(new DefaultBCAuthorizeRepository(new List<BCAuthorize>()));
        services.AddSingleton<IClaimProviderRepository>(new DefaultClaimProviderRepository(new List<ClaimProvider>()));
        services.AddSingleton<ICertificateAuthorityRepository>(new DefaultCertificateAuthorityRepository(new List<CertificateAuthority>()));
        services.AddSingleton<IClientRepository>(new DefaultClientRepository(new List<Client>()));
        services.AddSingleton<IGrantRepository>(new DefaultGrantRepository(new List<Consent>()));
        services.AddSingleton<IGroupRepository>(new DefaultGroupRepository(new List<Group>()));
        services.AddSingleton<ILanguageRepository>(new DefaultLanguageRepository(new List<SimpleIdServer.IdServer.Domains.Language>
        {
            LanguageBuilder.Build(SimpleIdServer.IdServer.Domains.Language.Default).AddDescription("English", "en").AddDescription("Anglais", "fr").Build()
        }));
        services.AddSingleton<IRealmRepository>(new DefaultRealmRepository(new List<Realm>()));
        services.AddSingleton<IRecurringJobStatusRepository>(new DefaultRecurringJobStatusRepository(new List<RecurringJobStatus>()));
        services.AddSingleton<IRegistrationWorkflowRepository>(new DefaultRegistrationWorkflowRepository(new List<RegistrationWorkflow>()));
        services.AddSingleton<IScopeRepository>(new DefaultScopeRepository(new List<Scope>()));
        services.AddSingleton<ITokenRepository>(new DefaultTokenRepository(new List<Token>()));
        services.AddSingleton<ITranslationRepository>(new DefaultTranslationRepository(new List<Translation>()));
        services.AddSingleton<IUmaPendingRequestRepository>(new DefaultUmaPendingRequestRepository(new List<UMAPendingRequest>()));
        services.AddSingleton<IUmaResourceRepository>(new DefaultUmaResourceRepository(new List<UMAResource>()));
        services.AddSingleton<IUserRepository>(new DefaultUserRepository(new List<User>()));
        services.AddSingleton<IUserSessionResitory>(new DefaultUserSessionRepository(new List<UserSession>()));
        services.AddSingleton<IDeviceAuthCodeRepository>(new DefaultDeviceAuthCodeRepository(new List<DeviceAuthCode>()));
        services.AddSingleton<IFileSerializedKeyStore>(new DefaultFileSerializedKeyStore(new List<SerializedFileKey>()));
        services.AddSingleton<IMessageBusErrorStore>(new DefaultMessageBusErrorStore(new List<MessageBusErrorMessage>()));
        services.AddSingleton<IIdentityProvisioningStore>(new DefaultIdentityProvisioningStore(new List<IdentityProvisioningDefinition>()));
        services.AddSingleton<IProvisioningStagingStore>(new DefaultProvisioningStagingStore(new List<ExtractedRepresentationStaging>()));
        services.AddTransient<ITransactionBuilder, DefaultTranslationBuilder>();
        services.AddTransient<IDataSeederExecutionHistoryRepository, DefaultDataSeederExecutionHistoryRepository>();
        return services;

    }

    private static IServiceCollection AddRegisterApi(this IServiceCollection services)
    {
        services.AddTransient<IRegisterClientRequestValidator, RegisterClientRequestValidator>();
        return services;
    }

    #endregion
}

public class RealmRoutePrefixConstraint : IRouteConstraint
{
    public bool Match(HttpContext? httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection) => true;
}

public class SidAuthCookie
{
    public SidAuthCookie()
    {
        Callback = (o) =>
        {
            o.LoginPath = $"/{Constants.AreaPwd}/Authenticate";
        };
    }

    internal Action<CookieAuthenticationOptions> Callback { get; set; }
}

public class SidHangfire
{
    public SidHangfire()
    {
        Callback = (o) =>
        {
            o.UseInMemoryStorage();
        };
    }

    internal Action<IGlobalConfiguration> Callback { get; set; }
}