// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Startup.Converters;
using SimpleIdServer.IdServer.Stores;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Startup.Conf.Migrations.AfterDeployment;

public class ConfigureAuthSchemeProviderDataSeeder : BaseAfterDeploymentDataSeeder
{
    private readonly ITransactionBuilder _transactionBuilder;
    private readonly IRealmRepository _realmRepository;
    private readonly IAuthenticationSchemeProviderRepository _authenticationSchemeProviderRepository;
    private readonly IAuthenticationSchemeProviderDefinitionRepository _authenticationSchemeProviderDefinitionRepository;

    public ConfigureAuthSchemeProviderDataSeeder(
        ITransactionBuilder transactionBuilder, 
        IRealmRepository realmRepository, 
        IAuthenticationSchemeProviderRepository authenticationSchemeProviderRepository,
        IAuthenticationSchemeProviderDefinitionRepository authenticationSchemeProviderDefinitionRepository,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
    {
        _transactionBuilder = transactionBuilder;
        _realmRepository = realmRepository;
        _authenticationSchemeProviderRepository = authenticationSchemeProviderRepository;
        _authenticationSchemeProviderDefinitionRepository = authenticationSchemeProviderDefinitionRepository;
    }

    public override string Name => nameof(ConfigureAuthSchemeProviderDataSeeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            var masterRealm = await _realmRepository.Get(Constants.DefaultRealm, cancellationToken);
            var existingDefinitions = await _authenticationSchemeProviderDefinitionRepository.GetAll(cancellationToken);
            var existingInstances = await _authenticationSchemeProviderRepository.GetAll(Constants.DefaultRealm, cancellationToken);
            var unknownDefinitions = AllAuthSchemeProviderDefinitions.Where(a => !existingDefinitions.Any(e => e.Name == a.Name)).ToList();
            var unknownInstances = AllAuthSchemeProviders.Where(a => !existingInstances.Any(e => e.Name == a.Name)).ToList();
            foreach(var unknownDefinition in unknownDefinitions)
            {
                _authenticationSchemeProviderDefinitionRepository.Add(unknownDefinition);
                existingDefinitions.Add(unknownDefinition);
            }

            foreach(var unknownInstance in unknownInstances)
            {
                unknownInstance.Realms = new List<Realm>
                {
                    masterRealm
                };
                unknownInstance.AuthSchemeProviderDefinition = existingDefinitions.First(e => e.Name == unknownInstance.AuthSchemeProviderDefinition.Name);
                _authenticationSchemeProviderRepository.Add(unknownInstance);
            }

            await transaction.Commit(cancellationToken);
        }
    }

    private static List<AuthenticationSchemeProvider> AllAuthSchemeProviders => new List<AuthenticationSchemeProvider>
    {
       AuthenticationSchemeProviderBuilder.Create(Facebook, "Facebook", "Facebook", "Facebook").Build(),
       AuthenticationSchemeProviderBuilder.Create(Google, "Google", "Google", "Google").Build(),
    };

    private static List<AuthenticationSchemeProviderDefinition> AllAuthSchemeProviderDefinitions => new List<AuthenticationSchemeProviderDefinition>
    {
        Facebook,
        Google
    };

    private static AuthenticationSchemeProviderDefinition Facebook = AuthenticationSchemeProviderDefinitionBuilder.Create("facebook", "Facebook", typeof(FacebookHandler), typeof(FacebookOptionsLite)).Build();
    private static AuthenticationSchemeProviderDefinition Google = AuthenticationSchemeProviderDefinitionBuilder.Create("google", "Google", typeof(GoogleHandler), typeof(GoogleOptionsLite)).Build();
}
