// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Middlewares;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        private readonly Helpers.IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly IdServerHostOptions _options;
        private readonly IDataProtectionProvider _dataProtection;
        private object _lck = new object();
        private readonly Dictionary<string, IDataProtector> _protections = new Dictionary<string, IDataProtector>();
        private IEnumerable<Domains.AuthenticationSchemeProvider> _cachedAuthenticationProviders;
        private DateTime? _nextExpirationTime;

        public DynamicAuthenticationSchemeProvider(
            Helpers.IHttpClientFactory httpClientFactory,
            IConfiguration configuration, 
            IServiceProvider serviceProvider, 
            IOptions<IdServerHostOptions> opts, 
            IDataProtectionProvider dataProtection, 
            IOptions<AuthenticationOptions> options) : base(options)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _options = opts.Value;
            _dataProtection = dataProtection;
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
            return provider == null ? null : Convert(provider);
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
                    var realm = RealmContext.Instance().Realm;
                    realm = realm ?? Constants.DefaultRealm;
                    authenticationSchemeProviders = await authenticationSchemeProviderRepository.GetAll(realm, CancellationToken.None);
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
            lock(_lck)
            {
                var handlerType = Type.GetType(provider.AuthSchemeProviderDefinition.HandlerFullQualifiedName);
                var authenticationHandlerType = GetGenericType(handlerType, typeof(AuthenticationHandler<>));
                if (authenticationHandlerType == null) return null;
                var liteOptionType = Assembly.GetEntryAssembly().GetType(provider.AuthSchemeProviderDefinition.OptionsFullQualifiedName);
                if (liteOptionType == null) return null;
                var optionType = authenticationHandlerType.GetGenericArguments().First();
                var liteOptionInterface = typeof(IDynamicAuthenticationOptions<>).MakeGenericType(optionType);
                var convert = liteOptionInterface.GetMethod("Convert");
                var section = _configuration.GetSection($"{provider.Name}:{liteOptionType.Name}");
                var liteOptions = section.Get(liteOptionType);
                if (liteOptions == null)
                    liteOptions = Activator.CreateInstance(liteOptionType);
                var options = convert.Invoke(liteOptions, new object[] { });
                PostConfigureOptions(optionType, handlerType, options);
                var optionsMonitorType = typeof(ConcreteOptionsMonitor<>).MakeGenericType(optionType);
                var optionsMonitor = Activator.CreateInstance(optionsMonitorType, options);
                return new SIDAuthenticationScheme(new AuthenticationScheme(provider.Name, provider.DisplayName, handlerType), optionsMonitor);
            }

            void PostConfigureOptions(Type optionType, Type handlerType, object options)
            {
                var signingSchemeProp = options.GetType().GetProperty("SignInScheme", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (signingSchemeProp != null)
                    signingSchemeProp.SetValue(options, Constants.DefaultExternalCookieAuthenticationScheme);
                var oauthOptions = options as OAuthOptions;
                if (oauthOptions != null)
                {
                    if (!_protections.ContainsKey(handlerType.FullName))
                        _protections.Add(handlerType.FullName, _dataProtection.CreateProtector(handlerType.FullName));
                    oauthOptions.DataProtectionProvider = _dataProtection;
                    oauthOptions.StateDataFormat = new PropertiesDataFormat(_protections[handlerType.FullName]);
                    if (oauthOptions.Backchannel == null)
                    {
                        oauthOptions.Backchannel = _httpClientFactory.GetHttpClient();
                        oauthOptions.Backchannel.DefaultRequestHeaders.UserAgent.ParseAdd("Microsoft ASP.NET Core OAuth handler");
                        oauthOptions.Backchannel.MaxResponseContentBufferSize = 1024 * 1024 * 10; // 10 MB
                    }
                }
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
