// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using SimpleIdServer.IdServer.Config;
using SimpleIdServer.IdServer.Stores;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Migrations;

public class InitLanguageDataSeeder : BaseAfterDeploymentDataSeeder
{
    private readonly ILanguageRepository _languageRepository;
    private readonly ITransactionBuilder _transactionBuilder;

    public InitLanguageDataSeeder(
        ILanguageRepository languageRepository,
        ITransactionBuilder transactionBuilder,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
    {
        _languageRepository = languageRepository;
        _transactionBuilder = transactionBuilder;
    }

    public override string Name => nameof(InitLanguageDataSeeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            var existingLanguages = await _languageRepository.GetAll(cancellationToken);
            var unknownLanguages = DefaultLanguages.All.Where(l => !existingLanguages.Any(el => el.Code == l.Code));
            foreach (var unknownLanguage in unknownLanguages)
            {
                _languageRepository.Add(unknownLanguage);
            }

            await transaction.Commit(cancellationToken);
        }
    }
}
