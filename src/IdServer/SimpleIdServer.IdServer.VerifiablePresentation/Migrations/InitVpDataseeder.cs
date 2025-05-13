// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using DataSeeder;
using FormBuilder.Repositories;
using FormBuilder.Stores;
using SimpleIdServer.IdServer.DataSeeder;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.VerifiablePresentation.Migrations;

public class InitVpDataseeder : BaseAuthDataSeeder
{
    private readonly ITransactionBuilder _transactionBuilder;

    public InitVpDataseeder(
        ITransactionBuilder transactionBuilder,
        IAuthenticationContextClassReferenceRepository acrRepository,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository,
        IFormStore formStore,
        IWorkflowStore workflowStore,
        IRealmRepository realmRepository) : base(acrRepository, dataSeederExecutionHistoryRepository, formStore, workflowStore, realmRepository)
    {
        _transactionBuilder = transactionBuilder;
    }

    public override string Name => nameof(InitVpDataseeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            await TryAddForm(IdServer.Constants.DefaultRealm, StandardVpRegisterForms.VpForm, cancellationToken);
            await Commit(cancellationToken);
            await transaction.Commit(cancellationToken);
        }
    }
}
