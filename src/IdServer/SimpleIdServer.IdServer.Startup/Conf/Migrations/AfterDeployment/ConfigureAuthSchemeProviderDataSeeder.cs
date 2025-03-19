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
