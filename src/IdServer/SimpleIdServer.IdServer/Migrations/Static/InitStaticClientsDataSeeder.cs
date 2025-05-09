// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Migrations.Static;

public class InitStaticClientsDataSeeder : BaseAfterDeploymentDataSeeder
{
    private readonly IClientRepository _clientRepository;
    private readonly StaticClientsDataSeeder _clientsData;
    private readonly ITransactionBuilder _transactionBuilder;

    public InitStaticClientsDataSeeder(
        IClientRepository clientRepository,
        ITransactionBuilder transactionBuilder,
        StaticClientsDataSeeder clientsData,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
    {
        _clientRepository = clientRepository;
        _clientsData = clientsData;
        _transactionBuilder = transactionBuilder;
    }

    public override string Name => nameof(InitStaticClientsDataSeeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            foreach(var client in _clientsData.Clients)
            {
                _clientRepository.Add(client);
            }

            await transaction.Commit(cancellationToken);
        }
    }
}

public class StaticClientsDataSeeder
{
    public StaticClientsDataSeeder(List<Client> clients)
    {
        Clients = clients;
    }

    public List<Client> Clients
    {
        get; private set;
    }
}