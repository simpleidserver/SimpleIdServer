// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using DataSeeder;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Config;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Migrations;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Swagger.Migrations;

public class ConfigureSwaggerDataseeder : BaseClientDataseeder
{
    private readonly ITransactionBuilder _transactionBuilder;
    private List<string> _redirectUrls;

    public ConfigureSwaggerDataseeder(ITransactionBuilder transactionBuilder, List<string> redirectUrls, IRealmRepository realmRepository, IScopeRepository scopeRepository, IClientRepository clientRepository, IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(realmRepository, scopeRepository, clientRepository, dataSeederExecutionHistoryRepository)
    {
        _transactionBuilder = transactionBuilder;
        _redirectUrls = redirectUrls;
    }

    public override string Name => nameof(ConfigureSwaggerDataseeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            await TryAddClient(BuildSwaggerClient(_redirectUrls.ToArray()), cancellationToken);
            await transaction.Commit(cancellationToken);
        }
    }

    private Client BuildSwaggerClient(string[] redirectUrls) => ClientBuilder.BuildTraditionalWebsiteClient("swaggerClient", "password", null, redirectUrls).AddScope(
            DefaultScopes.Provisioning,
            DefaultScopes.Users,
            DefaultScopes.Acrs,
            DefaultScopes.AuthenticationSchemeProviders,
            DefaultScopes.AuthenticationMethods,
            DefaultScopes.RegistrationWorkflows,
            DefaultScopes.ApiResources,
            DefaultScopes.Auditing,
            DefaultScopes.Scopes,
            DefaultScopes.CertificateAuthorities,
            DefaultScopes.Clients,
            DefaultScopes.Realms,
            DefaultScopes.Groups).Build();
}
