// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using FormBuilder.Models;
using FormBuilder.Stores;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Pwd.Migrations
{
    public class TransformNulltranslationToEmptyArrayDataseeder : BaseAfterDeploymentDataSeeder
    {
        private readonly ITransactionBuilder _transactionBuilder;
        private readonly IFormStore _formStore;

        public TransformNulltranslationToEmptyArrayDataseeder(
            ITransactionBuilder transactionBuilder,
            IFormStore formStore,
            IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
        {
            _transactionBuilder = transactionBuilder;
            _formStore = formStore;
        }


        public override string Name => nameof(TransformNulltranslationToEmptyArrayDataseeder);

        protected override async Task Execute(CancellationToken cancellationToken)
        {
            using (var transaction = _transactionBuilder.Build())
            {
                var forms = await _formStore.GetAll(Constants.DefaultRealm, cancellationToken);
                foreach (var form in forms)
                {
                    if (form.SuccessMessageTranslations == null)
                    {
                        form.SuccessMessageTranslations = new List<FormMessageTranslation>();
                    }

                    if(form.ErrorMessageTranslations == null)
                    {
                        form.ErrorMessageTranslations = new List<FormMessageTranslation>();
                    }
                }

                await transaction.Commit(cancellationToken);
                await _formStore.SaveChanges(cancellationToken);
            }
        }
    }
}
