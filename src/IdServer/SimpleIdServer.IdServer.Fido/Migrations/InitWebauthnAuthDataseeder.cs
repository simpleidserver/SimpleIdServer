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

public class InitWebauthnAuthDataseeder : BaseAuthDataSeeder
{
    private readonly ITransactionBuilder _transactionBuilder;

    public InitWebauthnAuthDataseeder(
        ITransactionBuilder transactionBuilder,
        IAuthenticationContextClassReferenceRepository acrRepository,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository,
        IFormStore formStore,
        IWorkflowStore workflowStore,
        IRealmRepository realmRepository) : base(acrRepository, dataSeederExecutionHistoryRepository, formStore, workflowStore, realmRepository)
    {
        _transactionBuilder = transactionBuilder;
    }

    public override string Name => nameof(InitWebauthnAuthDataseeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            var acr = BuildWebauthnAcr();
            await TryAddAcr(IdServer.Constants.DefaultRealm, acr, cancellationToken);
            await TryAddForm(IdServer.Constants.DefaultRealm, StandardFidoAuthForms.WebauthnForm, cancellationToken);
            await TryAddForm(IdServer.Constants.DefaultRealm, StandardFidoRegisterForms.WebauthnForm, cancellationToken);
            await TryAddWorkflow(IdServer.Constants.DefaultRealm, StandardFidoAuthWorkflows.DefaultWebauthnWorkflow, cancellationToken);
            await transaction.Commit(cancellationToken);
            await Commit(cancellationToken);
        }
    }

    private static AuthenticationContextClassReference BuildWebauthnAcr() => new AuthenticationContextClassReference
    {
        Id = Guid.NewGuid().ToString(),
        Name = "webauthn",
        DisplayName = "webauthn",
        UpdateDateTime = DateTime.UtcNow,
        AuthenticationWorkflow = StandardFidoAuthWorkflows.webauthnWorkflowId,
        Realms = new List<Realm>
        {
            Config.DefaultRealms.Master
        }
    };
}
