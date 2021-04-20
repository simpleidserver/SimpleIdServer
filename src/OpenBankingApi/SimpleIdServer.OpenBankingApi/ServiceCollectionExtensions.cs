using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NEventStore;
using SimpleIdServer.Jwt;
using SimpleIdServer.OAuth.Api.Authorization;
using SimpleIdServer.OAuth.Api.Authorization.ResponseModes;
using SimpleIdServer.OAuth.Api.Authorization.Validators;
using SimpleIdServer.OAuth.Api.Configuration;
using SimpleIdServer.OAuth.Api.Register.Handlers;
using SimpleIdServer.OAuth.Api.Token.TokenBuilders;
using SimpleIdServer.OpenBankingApi;
using SimpleIdServer.OpenBankingApi.Api.Authorization.ResponseModes;
using SimpleIdServer.OpenBankingApi.Api.Authorization.Validators;
using SimpleIdServer.OpenBankingApi.Api.BCAuthorize;
using SimpleIdServer.OpenBankingApi.Api.Configuration;
using SimpleIdServer.OpenBankingApi.Api.Register;
using SimpleIdServer.OpenBankingApi.Api.Token.TokenBuilders;
using SimpleIdServer.OpenBankingApi.Domains.Account;
using SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent;
using SimpleIdServer.OpenBankingApi.Infrastructure.Authorizations;
using SimpleIdServer.OpenBankingApi.Persistences;
using SimpleIdServer.OpenBankingApi.Persistences.InMemory;
using SimpleIdServer.OpenID.Api.BCAuthorize;
using SimpleIdServer.OpenID.Api.Token.TokenBuilders;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static OpenBankingApiBuilder AddOpenBankingApi(this IServiceCollection services, Action<OpenBankingApiOptions> callback = null)
        {
            if (callback != null)
            {
                services.Configure(callback);
            } 
            else
            {
                services.Configure<OpenBankingApiOptions>(opt => { });
            }

            services.AddJwt();

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IAuthorizationHandler, MtlsAccessTokenAuthorizationHandler>();

            services.AddTransient<IJob, EventStoreJob>();
            services.AddTransient<ICommandRepository, CommandRepository>();
            services.RegisterAllAssignableType(typeof(IEventHandler<>), typeof(IEventHandler<>).Assembly);

            services.RemoveAll<IAuthorizationRequestValidator>();
            services.AddTransient<IAuthorizationRequestValidator, OpenBankingApiAuthorizationRequestValidator>();

            services.RemoveAll<IClaimsJwsPayloadEnricher>();
            services.RemoveAll<ITokenBuilder>();
            services.RemoveAll<IBCAuthorizeHandler>();
            services.AddTransient<IClaimsJwsPayloadEnricher, OpenBankingApiClaimsJwsPayloadEnricher>();
            services.AddTransient<ITokenBuilder, OpenBankingApiIdTokenBuilder>();
            services.AddTransient<ITokenBuilder, OpenBankingApiAccessTokenBuilder>();
            services.AddTransient<ITokenBuilder, OpenIDRefreshTokenBuilder>();
            services.AddTransient<IBCAuthorizeHandler, OpenBankingBCAuthorizeHandler>();

            services.RemoveAll<IOAuthResponseMode>();
            services.RemoveAll<IOAuthResponseModeHandler>();
            services.AddTransient<IOAuthResponseMode, FragmentResponseModeHandler>();
            services.AddTransient<IOAuthResponseModeHandler, FragmentResponseModeHandler>();

            services.RemoveAll<IResponseModeHandler>();
            services.AddTransient<IResponseModeHandler, OpenBankingApiResponseModeHandler>();

            services.RemoveAll<IAddOAuthClientHandler>();
            services.AddTransient<IAddOAuthClientHandler, AddOpenBankingApiClientHandler>();

            services.RemoveAll<IConfigurationRequestHandler>();
            services.AddTransient<IConfigurationRequestHandler, OpenBankingApiConfigurationRequestHandler>();

            var accounts = new ConcurrentBag<AccountAggregate>();
            var accountAccessConsents = new ConcurrentBag<AccountAccessConsentAggregate>();
            var wireup = Wireup.Init().UsingInMemoryPersistence().Build();
            services.TryAddSingleton<IStoreEvents>(wireup);
            services.AddSingleton<IAccountRepository>(new InMemoryAccountRepository(accounts));
            services.AddSingleton<IAccountAccessConsentRepository>(new InMemoryAccountAccessConsentRepository(accountAccessConsents));
            return new OpenBankingApiBuilder(services);
        }

        public static IServiceCollection RegisterAllAssignableType<T>(this IServiceCollection services, Assembly assm)
        {
            return services.RegisterAllAssignableType(typeof(T), assm);
        }

        public static IServiceCollection RegisterAllAssignableType(this IServiceCollection services, Type type, Assembly assm, bool registerClass = false)
        {
            var types = assm.GetTypes().Where(p => type.IsAssignableFrom(p) || IsAssignableToGenericType(p, type));
            var addTransientMethod = typeof(ServiceCollectionServiceExtensions).GetMethods().FirstOrDefault(m =>
                m.Name == "AddTransient" &&
                m.IsGenericMethod == true &&
                m.GetGenericArguments().Count() == 2);
            var addTransientMethodClass = typeof(ServiceCollectionServiceExtensions).GetMethods().FirstOrDefault(m =>
                m.Name == "AddTransient" &&
                m.IsGenericMethod == false &&
                m.GetParameters().Count() == 2);
            foreach (var t in types)
            {
                if (t.IsInterface || t.IsAbstract)
                {
                    continue;
                }

                if (type.IsGenericTypeDefinition)
                {
                    var genericArgs = GetGenericArgs(t, type);
                    foreach (var args in genericArgs)
                    {
                        var genericType = type.MakeGenericType(args);
                        var method = addTransientMethod.MakeGenericMethod(new[] { genericType, t });
                        method.Invoke(services, new[] { services });
                        if (registerClass)
                        {
                            addTransientMethodClass.Invoke(services, new object[] { services, t });
                        }
                    }
                }
                else
                {
                    var method = addTransientMethod.MakeGenericMethod(new[] { type, t });
                    method.Invoke(services, new[] { services });
                    if (registerClass)
                    {
                        addTransientMethodClass.Invoke(services, new object[] { services, t });
                    }
                }
            }

            return services;
        }

        private static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            return GetGenericArgs(givenType, genericType).Any();
        }

        private static ICollection<Type[]> GetGenericArgs(Type givenType, Type genericType)
        {
            var result = new List<Type[]>();
            var interfaceTypes = givenType.GetInterfaces();
            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                {
                    result.Add(it.GetGenericArguments());
                }
            }

            return result;
        }
    }
}
