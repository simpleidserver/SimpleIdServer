// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using FormBuilder.Builders;
using FormBuilder.Repositories;
using FormBuilder.Stores;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Migrations;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Otp.Migrations;

public class InitOtpAuthDataseeder : BaseAuthDataSeeder
{
    private readonly ITransactionBuilder _transactionBuilder;

    protected InitOtpAuthDataseeder(
        ITransactionBuilder transactionBuilder,
        IAuthenticationContextClassReferenceRepository acrRepository,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository,
        IFormStore formStore,
        IWorkflowStore workflowStore) : base(acrRepository, dataSeederExecutionHistoryRepository, formStore, workflowStore)
    {
        _transactionBuilder = transactionBuilder;
    }

    public override string Name => nameof(InitOtpAuthDataseeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            var acr = BuildAcr();
            await TryAddAcr(IdServer.Constants.DefaultRealm, acr, cancellationToken);
            await TryAddForm(IdServer.Constants.DefaultRealm, StandardOtpAuthForms.OtpForm, cancellationToken);
            await TryAddWorkflow(IdServer.Constants.DefaultRealm, StandardOtpAuthWorkflows.DefaultWorkflow, cancellationToken);
            await Commit(cancellationToken);
            await transaction.Commit(cancellationToken);
        }
    }

    private static AuthenticationContextClassReference BuildAcr() => new AuthenticationContextClassReference
    {
        Id = Guid.NewGuid().ToString(),
        Name = "otp",
        DisplayName = "otp",
        UpdateDateTime = DateTime.UtcNow,
        Realms = new List<Realm>
        {
            Config.DefaultRealms.Master
        }
    };
}
