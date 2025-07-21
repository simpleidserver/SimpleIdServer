// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using DataSeeder;
using FormBuilder.Models;
using FormBuilder.Repositories;
using SimpleIdServer.IdServer.Console;
using SimpleIdServer.IdServer.Stores;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.DataSeeder
{
    public class UpdateTargetsConsoleWorkflowsDataseeder : BaseAfterDeploymentDataSeeder
    {
        private readonly ITransactionBuilder _transactionBuilder;
        private readonly IWorkflowStore _workflowStore;

        public UpdateTargetsConsoleWorkflowsDataseeder(
            ITransactionBuilder transactionBuilder,
            IWorkflowStore workflowStore,
            IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
        {
            _transactionBuilder = transactionBuilder;
            _workflowStore = workflowStore;
        }


        public override string Name => nameof(UpdateTargetsConsoleWorkflowsDataseeder);

        protected override async Task Execute(CancellationToken cancellationToken)
        {
            using (var transaction = _transactionBuilder.Build())
            {
                await TryUpdate(Constants.DefaultRealm, StandardConsoleAuthWorkflows.DefaultWorkflow, cancellationToken);
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
