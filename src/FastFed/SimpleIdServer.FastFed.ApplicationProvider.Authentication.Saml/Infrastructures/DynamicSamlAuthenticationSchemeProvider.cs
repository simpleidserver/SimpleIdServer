// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleIdServer.FastFed.Stores;
using SimpleIdServer.IdServer.Saml.Sp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.ApplicationProvider.Authentication.Saml.Infrastructures;

public interface ISamlAuthenticationSchemeProvider
{
    Task<SamlAuthenticationScheme> GetSamlSchemeAsync(string name);
}

public class DynamicSamlAuthenticationSchemeProvider : AuthenticationSchemeProvider, ISamlAuthenticationSchemeProvider
{
    private readonly IBusControl _busControl;
    private readonly IServiceProvider _serviceProvider;
    private readonly SamlAuthenticationOptions _samlAuthOptions;
    private DateTime? _nextExpirationTime;
    private IEnumerable<AuthSchemeProvider> _cachedAuthSchemeProviders;
    private object _lck = new object();

    public DynamicSamlAuthenticationSchemeProvider(
        IBusControl busControl,
        IServiceProvider serviceProvider,
        IOptions<SamlAuthenticationOptions> samlAuthOptions,
        IOptions<AuthenticationOptions> options) : base(options)
    {
        _busControl = busControl;
        _serviceProvider = serviceProvider;
        _samlAuthOptions = samlAuthOptions.Value;
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

    public override async Task<AuthenticationScheme> GetSchemeAsync(string name) => (await GetSamlSchemeAsync(name)).AuthScheme;

    public async Task<SamlAuthenticationScheme> GetSamlSchemeAsync(string name)
    {
        var result = await base.GetSchemeAsync(name);
        if (result != null)
            return new SamlAuthenticationScheme(result);

        var providers = await GetAuthenticationSchemeProviders();
        var provider = providers.FirstOrDefault(p => p.Name == name);
        return provider == null ? null : Convert(provider);
    }

    private SamlAuthenticationScheme Convert(AuthSchemeProvider provider)
    {
        lock (_lck)
        {
            var handlerType = typeof(SamlSpHandler);
            var options = new SamlSpOptions
            {
                SPId = _samlAuthOptions.SpId,
                IdpMetadataUrl = provider.SamlMetadataUri,
                SigningCertificate = _samlAuthOptions.SigningCertificate
            };
            if (options.Backchannel == null)
                options.Backchannel = new HttpClient(_samlAuthOptions.BackchannelHttpHandler ?? new HttpClientHandler());
            return new SamlAuthenticationScheme(new AuthenticationScheme(provider.Name, provider.DisplayName, handlerType), options);
        }
    }

    private async Task<IEnumerable<AuthSchemeProvider>> GetAuthenticationSchemeProviders()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var providerFederationStore = scope.ServiceProvider.GetRequiredService<IProviderFederationStore>();
            var currentDateTime = DateTime.UtcNow;
            var authenticationSchemeProviders = _cachedAuthSchemeProviders;
            if (_nextExpirationTime == null ||
                _nextExpirationTime.Value <= currentDateTime ||
                _samlAuthOptions.CacheSamlAuthProvidersInSeconds == null)
            {
                var result = new List<AuthSchemeProvider>();
                var providerFederations = await providerFederationStore.GetAll(CancellationToken.None);
                foreach (var providerFederation in providerFederations.Where(f => f.LastCapabilities != null && f.LastCapabilities.Status == Models.IdentityProviderStatus.CONFIRMED))
                {
                    var configuration = providerFederation.LastCapabilities.Configurations.SingleOrDefault(c => c.ProfileName == SimpleIdServer.FastFed.Authentication.Saml.Constants.ProvisioningProfileName);
                    if (configuration == null || string.IsNullOrWhiteSpace(configuration.IdProviderConfiguration)) continue;
                    var jObj = JsonObject.Parse(configuration.IdProviderConfiguration).AsObject();
                    if (jObj == null) continue;
                    if (jObj.ContainsKey(FastFed.Authentication.Saml.Constants.SamlMetadataUri))
                    {
                        result.Add(new AuthSchemeProvider
                        {
                            Name = providerFederation.EntityId,
                            DisplayName = providerFederation.DisplayName,
                            SamlMetadataUri = jObj[FastFed.Authentication.Saml.Constants.SamlMetadataUri].ToString()
                        });
                    }
                }

                authenticationSchemeProviders = result;
                if (_samlAuthOptions.CacheSamlAuthProvidersInSeconds != null)
                {
                    _nextExpirationTime = currentDateTime.AddSeconds(_samlAuthOptions.CacheSamlAuthProvidersInSeconds.Value);
                    _cachedAuthSchemeProviders = authenticationSchemeProviders;
                }
            }

            return authenticationSchemeProviders;
        }
    }
}