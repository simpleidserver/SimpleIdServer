// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using FormBuilder.Builders;
using FormBuilder.Repositories;
using FormBuilder.Stores;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Migrations;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Sms.Migrations;

public class InitSmsAuthDataseeder : BaseAuthDataSeeder
{
    private readonly ITransactionBuilder _transactionBuilder;
    private readonly IRegistrationWorkflowRepository _registrationWorkflowRepository;

    public InitSmsAuthDataseeder(
        ITransactionBuilder transactionBuilder,
        IRegistrationWorkflowRepository registrationWorkflowRepository,
        IAuthenticationContextClassReferenceRepository acrRepository,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository,
        IFormStore formStore,
        IWorkflowStore workflowStore) : base(acrRepository, dataSeederExecutionHistoryRepository, formStore, workflowStore)
    {
        _transactionBuilder = transactionBuilder;
        _registrationWorkflowRepository = registrationWorkflowRepository;
    }

    public override string Name => nameof(InitSmsAuthDataseeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            var acr = BuildAcr();
            await TryAddAcr(IdServer.Constants.DefaultRealm, acr, cancellationToken);
            await TryAddForm(IdServer.Constants.DefaultRealm, StandardSmsAuthForms.SmsForm, cancellationToken);
            await TryAddForm(IdServer.Constants.DefaultRealm, StandardSmsRegisterForms.SmsForm, cancellationToken);
            await TryAddWorkflow(IdServer.Constants.DefaultRealm, StandardSmsAuthWorkflows.DefaultWorkflow, cancellationToken);
            await Commit(cancellationToken);
            await transaction.Commit(cancellationToken);
        }
    }

    private static AuthenticationContextClassReference BuildAcr() => new AuthenticationContextClassReference
    {
        Id = Guid.NewGuid().ToString(),
        Name = "sms",
        DisplayName = "sms",
        UpdateDateTime = DateTime.UtcNow,
        Realms = new List<Realm>
        {
            SimpleIdServer.IdServer.Config.DefaultRealms.Master
        }
    };
}
