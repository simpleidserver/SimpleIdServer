// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using FormBuilder.Builders;
using FormBuilder.Repositories;
using FormBuilder.Stores;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Migrations;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Pwd.Migrations;

public abstract class InitPwdAuthDataseeder : BaseAuthDataSeeder
{
    private readonly ITransactionBuilder _transactionBuilder;
    private readonly IRegistrationWorkflowRepository _registrationWorkflowRepository;

    protected InitPwdAuthDataseeder(
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

    public override string Name => nameof(InitPwdAuthDataseeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            Config.DefaultAcrs.FirstLevelAssurance.AuthenticationWorkflow = StandardPwdAuthWorkflows.completePwdAuthWorkflowId;
            await TryAddAcr(Constants.DefaultRealm, Config.DefaultAcrs.FirstLevelAssurance, cancellationToken);
            await TryAddForm(Constants.DefaultRealm, StandardPwdAuthForms.PwdForm, cancellationToken);
            await TryAddForm(Constants.DefaultRealm, StandardPwdAuthForms.ResetForm, cancellationToken);
            await TryAddForm(Constants.DefaultRealm, StandardPwdAuthForms.ConfirmResetForm, cancellationToken);
            await TryAddForm(Constants.DefaultRealm, StandardPwdRegisterForms.PwdForm, cancellationToken);
            await TryAddWorkflow(Constants.DefaultRealm, StandardPwdAuthWorkflows.DefaultCompletePwdAuthWorkflow, cancellationToken);
            await TryAddWorkflow(Constants.DefaultRealm, StandardPwdAuthWorkflows.DefaultConfirmResetPwdWorkflow, cancellationToken);
            await TryAddWorkflow(Constants.DefaultRealm, StandardPwdRegistrationWorkflows.DefaultWorkflow, cancellationToken);
            var existingRegistrationWorkflow = await _registrationWorkflowRepository.GetByName(Constants.DefaultRealm, Constants.AreaPwd, cancellationToken);
            if (existingRegistrationWorkflow == null)
            {
                _registrationWorkflowRepository.Add(RegistrationWorkflowBuilder.New(Constants.AreaPwd, StandardPwdRegistrationWorkflows.workflowId).Build());
            }

            await transaction.Commit(cancellationToken);
            await Commit(cancellationToken);
        }
    }
}