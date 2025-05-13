// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Migrations;

public class LaunchMigrationFaultConsumer :
    IConsumer<Fault<MigrateApiScopesCommand>>,
    IConsumer<Fault<MigrateIdentityScopesCommand>>,
    IConsumer<Fault<MigrateApiResourcesCommand>>,
    IConsumer<Fault<MigrateClientsCommand>>,
    IConsumer<Fault<MigrateGroupsCommand>>,
    IConsumer<Fault<MigrateUsersCommand>>
{
    private readonly ITransactionBuilder _transactionBuilder;
    private readonly IMigrationStore _migrationStore;

    public LaunchMigrationFaultConsumer(
        ITransactionBuilder transactionBuilder,
        IMigrationStore migrationStore)
    {
        _transactionBuilder = transactionBuilder;
        _migrationStore = migrationStore;
    }

    public Task Consume(ConsumeContext<Fault<MigrateApiScopesCommand>> context)
    {
        return HandleError(MigrationExecutionHistoryTypes.APISCOPES, 
            context.Message.Message.Realm, 
            context.Message.Message.Name, 
            context.Message.Exceptions, 
            context.CancellationToken);
    }

    public Task Consume(ConsumeContext<Fault<MigrateIdentityScopesCommand>> context)
    {
        return HandleError(MigrationExecutionHistoryTypes.IDENTITYSCOPES,
            context.Message.Message.Realm,
            context.Message.Message.Name,
            context.Message.Exceptions,
            context.CancellationToken);
    }

    public Task Consume(ConsumeContext<Fault<MigrateApiResourcesCommand>> context)
    {
        return HandleError(MigrationExecutionHistoryTypes.APIRESOURCES,
            context.Message.Message.Realm,
            context.Message.Message.Name,
            context.Message.Exceptions,
            context.CancellationToken);
    }

    public Task Consume(ConsumeContext<Fault<MigrateClientsCommand>> context)
    {
        return HandleError(MigrationExecutionHistoryTypes.CLIENTS,
            context.Message.Message.Realm,
            context.Message.Message.Name,
            context.Message.Exceptions,
            context.CancellationToken);
    }

    public Task Consume(ConsumeContext<Fault<MigrateGroupsCommand>> context)
    {
        return HandleError(MigrationExecutionHistoryTypes.GROUPS,
            context.Message.Message.Realm,
            context.Message.Message.Name,
            context.Message.Exceptions,
            context.CancellationToken);
    }

    public Task Consume(ConsumeContext<Fault<MigrateUsersCommand>> context)
    {
        return HandleError(MigrationExecutionHistoryTypes.USERS,
            context.Message.Message.Realm,
            context.Message.Message.Name,
            context.Message.Exceptions,
            context.CancellationToken);
    }

    private async Task HandleError(MigrationExecutionHistoryTypes type, string realm, string name, ExceptionInfo[] exceptions, CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            var migrationExecution = await _migrationStore.Get(realm, name, cancellationToken);
            migrationExecution.LogErrors(type, exceptions.Select(e => e.StackTrace.ToString()).ToList());
            await transaction.Commit(cancellationToken);
        }
    }
}
