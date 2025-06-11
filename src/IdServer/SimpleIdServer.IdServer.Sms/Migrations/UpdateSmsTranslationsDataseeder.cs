// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using DataSeeder;
using FormBuilder.Models;
using FormBuilder.Stores;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Sms.Migrations
{
    public class UpdateSmsTranslationsDataseeder : BaseAfterDeploymentDataSeeder
    {
        private readonly ITransactionBuilder _transactionBuilder;
        private readonly IFormStore _formStore;

        public UpdateSmsTranslationsDataseeder(
            ITransactionBuilder transactionBuilder,
            IFormStore formStore,
            IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
        {
            _transactionBuilder = transactionBuilder;
            _formStore = formStore;
        }


        public override string Name => nameof(UpdateSmsTranslationsDataseeder);

        protected override async Task Execute(CancellationToken cancellationToken)
        {
            using (var transaction = _transactionBuilder.Build())
            {
                await TryUpdate(IdServer.Constants.DefaultRealm, StandardSmsAuthForms.SmsForm, cancellationToken);
                await TryUpdate(IdServer.Constants.DefaultRealm, StandardSmsRegisterForms.SmsForm, cancellationToken);
                await transaction.Commit(cancellationToken);
            }
        }

        private async Task TryUpdate(string realm, FormRecord formRecord, CancellationToken cancellationToken)
        {
            var existingForm = await _formStore.Get(realm, formRecord.Id, cancellationToken);
            if (existingForm == null)
            {
                return;
            }

            existingForm.SuccessMessageTranslations = formRecord.SuccessMessageTranslations;
            existingForm.ErrorMessageTranslations = formRecord.ErrorMessageTranslations;
            await _formStore.SaveChanges(cancellationToken);
        }
    }
}
