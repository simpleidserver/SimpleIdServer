// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using DataSeeder;
using FormBuilder.Stores;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Pwd.Migrations;

public class UpdatePwdFormTranslationDataseeder : BaseAfterDeploymentDataSeeder
{
    private readonly ITransactionBuilder _transactionBuilder;
    private readonly IFormStore _formStore;

    public UpdatePwdFormTranslationDataseeder(
        ITransactionBuilder transactionBuilder,
        IFormStore formStore,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
    {
        _transactionBuilder = transactionBuilder;
        _formStore = formStore;
    }


    public override string Name => nameof(UpdatePwdFormTranslationDataseeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {

            var existingForm = await _formStore.Get(Constants.DefaultRealm, StandardPwdAuthForms.PwdForm.Id, cancellationToken);
            if (existingForm == null)
            {
                return;
            }

            existingForm.ErrorMessageTranslations = StandardPwdAuthForms.PwdForm.ErrorMessageTranslations;
            await _formStore.SaveChanges(cancellationToken);
            await transaction.Commit(cancellationToken);
        }
    }
}
