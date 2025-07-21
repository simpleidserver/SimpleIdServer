// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using DataSeeder;
using FormBuilder.Builders;
using FormBuilder.Models;
using FormBuilder.Repositories;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.VerifiablePresentation.Migrations
{
    public class UpdateTargetsVpWorkflowsDataseeder : BaseAfterDeploymentDataSeeder
    {
        private readonly ITransactionBuilder _transactionBuilder;
        private readonly IWorkflowStore _workflowStore;

        public UpdateTargetsVpWorkflowsDataseeder(
            ITransactionBuilder transactionBuilder,
            IWorkflowStore workflowStore,
            IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
        {
            _transactionBuilder = transactionBuilder;
            _workflowStore = workflowStore;
        }


        public override string Name => nameof(UpdateTargetsVpWorkflowsDataseeder);

        protected override async Task Execute(CancellationToken cancellationToken)
        {
            using (var transaction = _transactionBuilder.Build())
            {
                await TryUpdate(IdServer.Constants.DefaultRealm, StandardVpRegistrationWorkflows.DefaultWorkflow, cancellationToken);
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
