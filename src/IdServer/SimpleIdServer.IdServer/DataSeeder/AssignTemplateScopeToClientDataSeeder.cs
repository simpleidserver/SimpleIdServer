// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using SimpleIdServer.IdServer.Config;
using SimpleIdServer.IdServer.Stores;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.DataSeeder;

public class AssignTemplateScopeToClientDataSeeder : BaseAfterDeploymentDataSeeder
{
    private readonly IClientRepository _clientRepository;
    private readonly ITransactionBuilder _transactionBuilder;
    private readonly IScopeRepository _scopeRepository;

    public AssignTemplateScopeToClientDataSeeder(
        IClientRepository clientRepository, 
        ITransactionBuilder transactionBuilder,
        IScopeRepository scopeRepository,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
    {
        _clientRepository = clientRepository;
        _transactionBuilder = transactionBuilder;
        _scopeRepository = scopeRepository;
    }

    public override string Name => nameof(AssignTemplateScopeToClientDataSeeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            var client = await _clientRepository.GetByClientId(Constants.DefaultRealm, DefaultClients.SidAdminClientId, cancellationToken);
            if (client != null && !client.Scopes.Any(s => s.Name == DefaultScopes.Templates.Name))
            {
                var scope = await _scopeRepository.GetByName(Constants.DefaultRealm, DefaultScopes.Templates.Name, cancellationToken);
                if (scope != null)
                {
                    client.Scopes.Add(scope);
                }
            }

            await transaction.Commit(cancellationToken);
        }
    }
}
