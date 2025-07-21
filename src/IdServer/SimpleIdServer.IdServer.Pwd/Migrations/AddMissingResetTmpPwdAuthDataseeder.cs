// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using DataSeeder;
using FormBuilder.Repositories;
using FormBuilder.Stores;
using SimpleIdServer.IdServer.DataSeeder;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Pwd.Migrations
{
    public class AddMissingResetTmpPwdAuthDataseeder : BaseAuthDataSeeder
    {
        private readonly ITransactionBuilder _transactionBuilder;

        public AddMissingResetTmpPwdAuthDataseeder(
            ITransactionBuilder transactionBuilder,
            IRegistrationWorkflowRepository registrationWorkflowRepository,
            IAuthenticationContextClassReferenceRepository acrRepository,
            IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository,
            IFormStore formStore,
            IWorkflowStore workflowStore,
            IRealmRepository realmRepository) : base(acrRepository, dataSeederExecutionHistoryRepository, formStore, workflowStore, realmRepository)
        {
            _transactionBuilder = transactionBuilder;
        }


        public override string Name => nameof(AddMissingResetTmpPwdAuthDataseeder);

        protected override async Task Execute(CancellationToken cancellationToken)
        {
            using (var transaction = _transactionBuilder.Build())
            {
                await TryAddForm(Constants.DefaultRealm, StandardPwdAuthForms.ResetTemporaryPasswordForm, cancellationToken);
                await transaction.Commit(cancellationToken);
                await Commit(cancellationToken);
            }
        }
    }
}
