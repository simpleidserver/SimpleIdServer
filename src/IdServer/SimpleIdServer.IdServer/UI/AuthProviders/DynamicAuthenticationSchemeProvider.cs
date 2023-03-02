// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.UI.AuthProviders
{
    public interface ISIDAuthenticationSchemeProvider
    {
        Task<SIDAuthenticationScheme> GetSIDSchemeAsync(string name);
    }

    public class SIDAuthenticationScheme
    {
        public SIDAuthenticationScheme(AuthenticationScheme authScheme)
        {
            AuthScheme = authScheme;
        }

        public SIDAuthenticationScheme(AuthenticationScheme authScheme, object optionsMonitor) : this(authScheme)
        {
            OptionsMonitor = optionsMonitor;
        }


        public AuthenticationScheme AuthScheme { get; set; }
        public object OptionsMonitor { get; set; }
    }

    public class DynamicAuthenticationSchemeProvider : AuthenticationSchemeProvider, ISIDAuthenticationSchemeProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IdServerHostOptions _options;
        private IEnumerable<Domains.AuthenticationSchemeProvider> _cachedAuthenticationProviders;
        private DateTime? _nextExpirationTime;

        public DynamicAuthenticationSchemeProvider(IServiceProvider serviceProvider, IOptions<IdServerHostOptions> opts, IOptions<AuthenticationOptions> options) : base(options)
        {
            _serviceProvider = serviceProvider;
            _options = opts.Value;
        }

        public async override Task<IEnumerable<AuthenticationScheme>> GetAllSchemesAsync()
        {
            var rules = (await base.GetAllSchemesAsync()).ToList();
            var authenticationSchemeProviders = await GetAuthenticationSchemeProviders();
            foreach (var scheme in authenticationSchemeProviders)
            {
                var newRule = Convert(scheme);
                if (newRule == null)
                    continue;

                rules.Add(newRule.AuthScheme);
            }

            return rules;
        }

        public override Task<IEnumerable<AuthenticationScheme>> GetRequestHandlerSchemesAsync() => GetAllSchemesAsync();

        public override async Task<AuthenticationScheme> GetSchemeAsync(string name) => (await GetSIDSchemeAsync(name)).AuthScheme;

        public async Task<SIDAuthenticationScheme> GetSIDSchemeAsync(string name)
        {
            var result = await base.GetSchemeAsync(name);
            if (result != null)
                return new SIDAuthenticationScheme(result);

            var providers = await GetAuthenticationSchemeProviders();
            var provider = providers.FirstOrDefault(p => p.Name == name);
            return providers == null ? null : Convert(provider);
        }

        private async Task<IEnumerable<Domains.AuthenticationSchemeProvider>> GetAuthenticationSchemeProviders()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var authenticationSchemeProviderRepository = scope.ServiceProvider.GetRequiredService<IAuthenticationSchemeProviderRepository>();
                var currentDateTime = DateTime.UtcNow;
                var authenticationSchemeProviders = _cachedAuthenticationProviders;
                if (_nextExpirationTime == null ||
                    _nextExpirationTime.Value <= currentDateTime ||
                    _options.CacheExternalAuthProvidersInSeconds == null)
                {
                    authenticationSchemeProviders = await authenticationSchemeProviderRepository.Query().Include(c => c.Properties)
                        .Include(c => c.AuthSchemeProviderDefinition).ThenInclude(c => c.Properties).ToListAsync(CancellationToken.None);
                    if (_options.CacheExternalAuthProvidersInSeconds != null)
                    {
                        _nextExpirationTime = currentDateTime.AddSeconds(_options.CacheExternalAuthProvidersInSeconds.Value);
                        _cachedAuthenticationProviders = authenticationSchemeProviders;
                    }
                }

                return authenticationSchemeProviders;
            }
        }

        private SIDAuthenticationScheme Convert(Domains.AuthenticationSchemeProvider provider)
        {
            var handlerType = Type.GetType(provider.AuthSchemeProviderDefinition.HandlerFullQualifiedName);
            var authenticationHandlerType = GetGenericType(handlerType, typeof(AuthenticationHandler<>));
            if (authenticationHandlerType == null) return null;
            var liteOptionType = Type.GetType(provider.AuthSchemeProviderDefinition.OptionsFullQualifiedName);
            if (liteOptionType == null) return null;

            var optionType = authenticationHandlerType.GetGenericArguments().First();
            var liteOptionInterface = typeof(IDynamicAuthenticationOptions<>).MakeGenericType(optionType);
            var convert = liteOptionInterface.GetMethod("Convert");
            var liteOptions = AuthenticationSchemeSerializer.DeserializeOptions(liteOptionType, provider.Properties);
            var options = convert.Invoke(liteOptions, new object[] { });
            PostConfigureOptions(optionType, handlerType, options);
            var optionsMonitorType = typeof(ConcreteOptionsMonitor<>).MakeGenericType(optionType);
            var optionsMonitor = Activator.CreateInstance(optionsMonitorType, options);
            return new SIDAuthenticationScheme(new AuthenticationScheme(provider.Name, provider.DisplayName, handlerType), optionsMonitor);

            void PostConfigureOptions(Type optionType, Type handlerType, object options)
            {
                var postConfigureOptionsType = typeof(OAuthPostConfigureOptions<,>).MakeGenericType(optionType, handlerType);
                var constructor = postConfigureOptionsType.GetConstructors().First();
                var args = new List<object>();
                foreach (var parameter in constructor.GetParameters())
                    args.Add(_serviceProvider.GetRequiredService(parameter.ParameterType));

                var postConfigure = postConfigureOptionsType.GetMethod("PostConfigure", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                var instance = Activator.CreateInstance(postConfigureOptionsType, args.ToArray());
                postConfigure.Invoke(instance, new object[] { provider.Name, options });
            }
        }

        public static Type GetGenericType(Type givenType, Type genericType)
        {
            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return givenType;

            Type baseType = givenType.BaseType;
            if (baseType == null)
                return null;

            return GetGenericType(baseType, genericType);
        }

        private class ConcreteOptionsMonitor<T> : IOptionsMonitor<T> where T : class
        {
            public ConcreteOptionsMonitor(T value)
            {
                CurrentValue = value;
            }

            public T CurrentValue { get; private set; }

            public T Get(string name)
            {
                return CurrentValue;
            }

            public IDisposable OnChange(Action<T, string> listener)
            {
                return null;
            }
        }
    }
}
