// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using FormBuilder.Stores;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Pwd.Migrations
{
    public class UpdatePwdFormDataseeder : BaseAfterDeploymentDataSeeder
    {
        private readonly ITransactionBuilder _transactionBuilder;
        private readonly IFormStore _formStore;

        public UpdatePwdFormDataseeder(
            ITransactionBuilder transactionBuilder,
            IFormStore formStore,
            IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
        {
            _transactionBuilder = transactionBuilder;
            _formStore = formStore;
        }


        public override string Name => nameof(UpdatePwdFormDataseeder);

        protected override async Task Execute(CancellationToken cancellationToken)
        {
            using (var transaction = _transactionBuilder.Build())
            {
                var existingForm = await _formStore.Get(IdServer.Constants.DefaultRealm, StandardPwdAuthForms.PwdForm.Id, cancellationToken);
                if (existingForm == null)
                {
                    return;
                }

                existingForm.Elements = StandardPwdAuthForms.PwdForm.Elements;
                await _formStore.SaveChanges(cancellationToken);
                await transaction.Commit(cancellationToken);
            }
        }
    }
}
