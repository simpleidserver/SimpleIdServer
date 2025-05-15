// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using SimpleIdServer.IdServer.Stores;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.DataSeeder;

public class UpdateOtherLifetimeDataSeeder : BaseAfterDeploymentDataSeeder
{
    private readonly ITransactionBuilder _transactionBuilder;
    private readonly IClientRepository _clientRepository;

    public UpdateOtherLifetimeDataSeeder(
        ITransactionBuilder transactionBuilder,
        IClientRepository clientRepository,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
    {
        _transactionBuilder = transactionBuilder;
        _clientRepository = clientRepository;
    }

    public override string Name => nameof(UpdateOtherLifetimeDataSeeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            var clients = await _clientRepository.GetAll(Constants.DefaultRealm, cancellationToken);
            foreach (var cl in clients)
            {
                if (cl.MaxRequestParameterLifetimeSeconds == default)
                {
                    cl.MaxRequestParameterLifetimeSeconds = 300;
                }

                if (cl.MaxBindingMessageSize == default)
                {
                    cl.MaxBindingMessageSize = 150;
                }

                if (cl.DpopLifetimeSeconds == default)
                {
                    cl.DpopLifetimeSeconds = 300;
                }

                _clientRepository.Update(cl);
            }

            await transaction.Commit(cancellationToken);
        }
    }
}
