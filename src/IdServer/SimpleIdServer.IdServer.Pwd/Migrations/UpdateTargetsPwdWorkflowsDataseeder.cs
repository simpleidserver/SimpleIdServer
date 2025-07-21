// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using DataSeeder;
using FormBuilder.Builders;
using FormBuilder.Models;
using FormBuilder.Repositories;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Pwd.Migrations
{
    public class UpdateTargetsPwdWorkflowsDataseeder : BaseAfterDeploymentDataSeeder
    {
        private readonly ITransactionBuilder _transactionBuilder;
        private readonly IWorkflowStore _workflowStore;

        public UpdateTargetsPwdWorkflowsDataseeder(
            ITransactionBuilder transactionBuilder,
            IWorkflowStore workflowStore,
            IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
        {
            _transactionBuilder = transactionBuilder;
            _workflowStore = workflowStore;
        }


        public override string Name => nameof(UpdateTargetsPwdWorkflowsDataseeder);

        protected override async Task Execute(CancellationToken cancellationToken)
        {
            using (var transaction = _transactionBuilder.Build())
            {
                await TryUpdate(Constants.DefaultRealm, StandardPwdAuthWorkflows.DefaultPwdWorkflow, cancellationToken);
                await TryUpdate(Constants.DefaultRealm, StandardPwdAuthWorkflows.DefaultConfirmResetPwdWorkflow, cancellationToken);
                await TryUpdate(Constants.DefaultRealm, StandardPwdAuthWorkflows.DefaultCompletePwdAuthWorkflow, cancellationToken);
                await TryUpdate(Constants.DefaultRealm, StandardPwdRegistrationWorkflows.DefaultWorkflow, cancellationToken);
                await transaction.Commit(cancellationToken);
            }
        }

        private async Task TryUpdate(string realm, WorkflowRecord workflowRecord, CancellationToken cancellationToken)
        {
            var existingWorkflow = await _workflowStore.Get(realm, workflowRecord.Id, cancellationToken);
            if (existingWorkflow == null)
            {
                return;
            }

            existingWorkflow.Steps = workflowRecord.Steps;
            existingWorkflow.Links = workflowRecord.Links;
            await _workflowStore.SaveChanges(cancellationToken);
        }
    }
}
