// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using DataSeeder;
using FormBuilder.Builders;
using FormBuilder.Repositories;
using FormBuilder.Stores;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Migrations;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Fido.Migrations;

public class InitMobileAuthDataseeder : BaseAuthDataSeeder
{
    private readonly ITransactionBuilder _transactionBuilder;

    public InitMobileAuthDataseeder(
        ITransactionBuilder transactionBuilder,
        IAuthenticationContextClassReferenceRepository acrRepository,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository,
        IFormStore formStore,
        IWorkflowStore workflowStore,
        IRealmRepository realmRepository) : base(acrRepository, dataSeederExecutionHistoryRepository, formStore, workflowStore, realmRepository)
    {
        _transactionBuilder = transactionBuilder;
    }

    public override string Name => nameof(InitMobileAuthDataseeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            var acr = BuildMobileAcr();
            await TryAddAcr(IdServer.Constants.DefaultRealm, acr, cancellationToken);
            await TryAddForm(IdServer.Constants.DefaultRealm, StandardFidoAuthForms.MobileForm, cancellationToken);
            await TryAddForm(IdServer.Constants.DefaultRealm, StandardFidoRegisterForms.MobileForm, cancellationToken);
            await TryAddWorkflow(IdServer.Constants.DefaultRealm, StandardFidoAuthWorkflows.DefaultMobileWorkflow, cancellationToken);
            await transaction.Commit(cancellationToken);
            await Commit(cancellationToken);
        }
    }

    private static AuthenticationContextClassReference BuildMobileAcr() => new AuthenticationContextClassReference
    {
        Id = Guid.NewGuid().ToString(),
        Name = "mobile",
        DisplayName = "mobile",
        UpdateDateTime = DateTime.UtcNow,
        AuthenticationWorkflow = StandardFidoAuthWorkflows.mobileWorkflowId,
        Realms = new List<Realm>
        {
            SimpleIdServer.IdServer.Config.DefaultRealms.Master
        }
    };
}
