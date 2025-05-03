// Copyright (c) SimpleIdServer. AllClients rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Config;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IdServerLogging.Conf.Migrations.AfterDeployment;

public class ConfigureCredentialIssuerDataSeeder : BaseAfterDeploymentDataSeeder
{
    private readonly ITransactionBuilder _transactionBuilder;
    private readonly IClientRepository _clientRepository;
    private readonly IRealmRepository _realmRepository;
    private readonly IScopeRepository _scopeRepository;
    private readonly IPresentationDefinitionStore _presentationDefinitionStore;

    public ConfigureCredentialIssuerDataSeeder(ITransactionBuilder transactionBuilder,
        IClientRepository clientRepository,
        IRealmRepository realmRepository,
        IScopeRepository scopeRepository,
        IPresentationDefinitionStore presentationDefinitionStore,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
    {
        _transactionBuilder = transactionBuilder;
        _clientRepository = clientRepository;
        _realmRepository = realmRepository;
        _scopeRepository = scopeRepository;
        _presentationDefinitionStore = presentationDefinitionStore;
    }

    public override string Name => nameof(ConfigureCredentialIssuerDataSeeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using(var transaction = _transactionBuilder.Build())
        {
            var masterRealm = await _realmRepository.Get(SimpleIdServer.IdServer.Constants.DefaultRealm, cancellationToken);
            var scopeNames = AllClients.SelectMany(c => c.Scopes.Select(s => s.Name)).Distinct().ToList();
            var clientIds = AllClients.Select(c => c.ClientId).ToList();
            var existingScopes = await _scopeRepository.GetByNames(SimpleIdServer.IdServer.Constants.DefaultRealm, scopeNames, cancellationToken);
            var existingClients = await _clientRepository.GetByClientIds(SimpleIdServer.IdServer.Constants.DefaultRealm, clientIds, cancellationToken);
            var university = UniversityDegree;
            var existingPresentation = await _presentationDefinitionStore.GetByPublicId(university.PublicId, SimpleIdServer.IdServer.Constants.DefaultRealm, cancellationToken);
            foreach (var scope in AllScopes)
            {
                var existingScope = existingScopes.SingleOrDefault(s => s.Name == scope.Name);
                if (existingScope == null)
                {
                    scope.Realms = new List<Realm>
                    {
                        masterRealm
                    };
                    existingScopes.Add(scope);
                    _scopeRepository.Add(scope);
                }
            }

            foreach (var client in AllClients)
            {
                var existingClient = existingClients.SingleOrDefault(c => c.ClientId == client.ClientId);
                if(existingClient != null)
                {
                    continue;
                }

                client.Realms = new List<Realm>
                {
                    masterRealm
                };
                client.Scopes = existingScopes.Where(s => client.Scopes.Any(cs => cs.Name == s.Name)).ToList();
                _clientRepository.Add(client);
            }

            if(existingPresentation == null)
            {
                _presentationDefinitionStore.Add(university);
            }

            await transaction.Commit(cancellationToken);
        }
    }

    private List<Client> AllClients => new List<Client>
    {
        CredentialIssuerAdminWebsite,
        CredentialIssuerApi
    };

    private List<Scope> AllScopes => new List<Scope>
    {
        UniversityDegreeScope,
        CtWalletScope
    };

    private static Client CredentialIssuerAdminWebsite => ClientBuilder.BuildTraditionalWebsiteClient("CredentialIssuer-manager", "password", null, "https://localhost:5006/*", "https://credentialissuerwebsite.simpleidserver.com/*", "https://credentialissuerwebsite.localhost.com/*", "http://credentialissuerwebsite.localhost.com/*", "https://credentialissuerwebsite.sid.svc.cluster.local/*")
            .EnableClientGrantType()
            .SetRequestObjectEncryption()
            .AddPostLogoutUri("https://localhost:5006/signout-callback-oidc").AddPostLogoutUri("https://credissuer-website.sid.svc.cluster.local/signout-callback-oidc").AddPostLogoutUri("https://website.simpleidserver.com/signout-callback-oidc")
            .AddAuthDataTypes("photo")
            .SetClientName("Credential issuer manager")
            .SetBackChannelLogoutUrl("https://localhost:5006/bc-logout")
            .SetClientLogoUri("https://cdn.logo.com/hotlink-ok/logo-social.png")
            .AddScope(
                DefaultScopes.OpenIdScope,
                DefaultScopes.Profile,
                DefaultScopes.CredentialConfigurations,
                DefaultScopes.CredentialInstances,
                DefaultScopes.DeferredCreds).Build();

    private static Client CredentialIssuerApi => ClientBuilder.BuildCredentialIssuer("CredentialIssuer", "password", null, "https://38a0-81-246-134-116.ngrok-free.app/signin-oidc", "https://localhost:5005/*", "http://localhost:5005/*", "https://credentialissuer.simpleidserver.com/*", "https://credentialissuer.localhost.com/*", "https://credentialissuer.sid.svc.cluster.local/*")
            .SetClientName("Credential issuer")
            .AddScope(
                DefaultScopes.OpenIdScope,
                DefaultScopes.Profile,
                UniversityDegreeScope,
                CtWalletScope).IsTransactionCodeRequired().Build();

    private static Scope UniversityDegreeScope = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Name = "university_degree",
        Realms = new List<Realm>
        {
            DefaultRealms.Master
        },
        Type = ScopeTypes.APIRESOURCE,
        Protocol = ScopeProtocols.OAUTH,
        IsExposedInConfigurationEdp = true,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };

    private static Scope CtWalletScope = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Name = "ct_wallet",
        Realms = new List<Realm>
        {
            DefaultRealms.Master
        },
        Type = ScopeTypes.APIRESOURCE,
        Protocol = ScopeProtocols.OAUTH,
        IsExposedInConfigurationEdp = true,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };

    private static PresentationDefinition UniversityDegree = PresentationDefinitionBuilder.New("universitydegree_vp", "University Degree").AddLdpVcInputDescriptor("UniversityDegree", "UniversityDegree", "UniversityDegree").Build();
}
