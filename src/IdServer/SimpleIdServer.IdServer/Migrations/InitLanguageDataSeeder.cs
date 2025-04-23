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
    private readonly ITranslationRepository _translationRepository;

    public InitLanguageDataSeeder(
        ILanguageRepository languageRepository,
        ITransactionBuilder transactionBuilder,
        ITranslationRepository translationRepository,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
    {
        _languageRepository = languageRepository;
        _transactionBuilder = transactionBuilder;
        _translationRepository = translationRepository;
    }

    public override string Name => nameof(InitLanguageDataSeeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            var existingLanguages = await _languageRepository.GetAll(cancellationToken);
            foreach(var language in DefaultLanguages.All)
            {
                var existingLanguage = existingLanguages.SingleOrDefault(l => l.Code == language.Code);
                if (existingLanguage != null)
                {
                    var unknownDescriptions = language.Descriptions.Where(d => !existingLanguage.Descriptions.Any(ed => ed.Language == d.Language));
                    foreach (var unknownDescription in unknownDescriptions)
                    {
                        _translationRepository.Add(unknownDescription);
                    }
                }
                else
                {
                    _languageRepository.Add(language);
                }
            }

            await transaction.Commit(cancellationToken);
        }
    }
}
