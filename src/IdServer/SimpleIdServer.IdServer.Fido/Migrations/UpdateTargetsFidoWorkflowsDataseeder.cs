// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using DataSeeder;
using FormBuilder.Builders;
using FormBuilder.Models;
using FormBuilder.Repositories;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Fido.Migrations
{
    public class UpdateTargetsFidoWorkflowsDataseeder : BaseAfterDeploymentDataSeeder
    {
        private readonly ITransactionBuilder _transactionBuilder;
        private readonly IWorkflowStore _workflowStore;

        public UpdateTargetsFidoWorkflowsDataseeder(
            ITransactionBuilder transactionBuilder,
            IWorkflowStore workflowStore,
            IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
        {
            _transactionBuilder = transactionBuilder;
            _workflowStore = workflowStore;
        }


        public override string Name => nameof(UpdateTargetsFidoWorkflowsDataseeder);

        protected override async Task Execute(CancellationToken cancellationToken)
        {
            using (var transaction = _transactionBuilder.Build())
            {
                await TryUpdate(IdServer.Constants.DefaultRealm, StandardFidoAuthWorkflows.DefaultWebauthnWorkflow, cancellationToken);
                await TryUpdate(IdServer.Constants.DefaultRealm, StandardFidoAuthWorkflows.DefaultMobileWorkflow, cancellationToken);
                await TryUpdate(IdServer.Constants.DefaultRealm, StandardFidoRegistrationWorkflows.WebauthnWorkflow, cancellationToken);
                await TryUpdate(IdServer.Constants.DefaultRealm, StandardFidoRegistrationWorkflows.MobileWorkflow, cancellationToken);
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
