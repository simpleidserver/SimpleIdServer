// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using DataSeeder;
using FormBuilder.Repositories;
using FormBuilder.Stores;
using SimpleIdServer.IdServer.Console;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.DataSeeder;

public class InitConsoleAuthDataseeder : BaseAuthDataSeeder
{
    private readonly ITransactionBuilder _transactionBuilder;

    public InitConsoleAuthDataseeder(
        ITransactionBuilder transactionBuilder,
        IRealmRepository realmRepository,
        IAuthenticationContextClassReferenceRepository acrRepository,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository,
        IFormStore formStore,
        IWorkflowStore workflowStore) : base(acrRepository, dataSeederExecutionHistoryRepository, formStore, workflowStore, realmRepository)
    {
        _transactionBuilder = transactionBuilder;
    }

    public override string Name => nameof(InitConsoleAuthDataseeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            var acr = BuildAcr();
            await TryAddAcr(IdServer.Constants.DefaultRealm, acr, cancellationToken);
            await TryAddForm(IdServer.Constants.DefaultRealm, StandardConsoleAuthForms.ConsoleForm, cancellationToken);
            await TryAddWorkflow(IdServer.Constants.DefaultRealm, StandardConsoleAuthWorkflows.DefaultWorkflow, cancellationToken);
            await Commit(cancellationToken);
            await transaction.Commit(cancellationToken);
        }
    }

    private static AuthenticationContextClassReference BuildAcr() => new AuthenticationContextClassReference
    {
        Id = Guid.NewGuid().ToString(),
        Name = "console",
        DisplayName = "Console",
        UpdateDateTime = DateTime.UtcNow,
        AuthenticationWorkflow = StandardConsoleAuthWorkflows.workflowId,
        Realms = new List<Realm>
        {
            Config.DefaultRealms.Master
        }
    };
}
