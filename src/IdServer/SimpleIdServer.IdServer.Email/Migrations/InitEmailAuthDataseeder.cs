// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using DataSeeder;
using FormBuilder.Builders;
using FormBuilder.Repositories;
using FormBuilder.Stores;
using SimpleIdServer.IdServer.DataSeeder;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Email.Migrations;

public class InitEmailAuthDataseeder : BaseAuthDataSeeder
{
    private readonly ITransactionBuilder _transactionBuilder;

    public InitEmailAuthDataseeder(
        ITransactionBuilder transactionBuilder,
        IRealmRepository realmRepository,
        IAuthenticationContextClassReferenceRepository acrRepository,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository,
        IFormStore formStore,
        IWorkflowStore workflowStore) : base(acrRepository, dataSeederExecutionHistoryRepository, formStore, workflowStore, realmRepository)
    {
        _transactionBuilder = transactionBuilder;
    }

    public override string Name => nameof(InitEmailAuthDataseeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            var acr = BuildAcr();
            await TryAddAcr(IdServer.Constants.DefaultRealm, acr, cancellationToken);
            await TryAddForm(IdServer.Constants.DefaultRealm, StandardEmailAuthForms.EmailForm, cancellationToken);
            await TryAddForm(IdServer.Constants.DefaultRealm, StandardEmailAuthForms.EmptyEmailForm, cancellationToken);
            await TryAddForm(IdServer.Constants.DefaultRealm, StandardEmailRegistrationForms.EmailForm, cancellationToken);
            await TryAddWorkflow(IdServer.Constants.DefaultRealm, StandardEmailAuthWorkflows.DefaultWorkflow, cancellationToken);
            await Commit(cancellationToken);
            await transaction.Commit(cancellationToken);
        }
    }

    private static AuthenticationContextClassReference BuildAcr() => new AuthenticationContextClassReference
    {
        Id = Guid.NewGuid().ToString(),
        Name = "email",
        DisplayName = "Email",
        UpdateDateTime = DateTime.UtcNow,
        AuthenticationWorkflow = StandardEmailAuthWorkflows.workflowId,
        Realms = new List<Realm>
        {
            Config.DefaultRealms.Master
        }
    };
}
