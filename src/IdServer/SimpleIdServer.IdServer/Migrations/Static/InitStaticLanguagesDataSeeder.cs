// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Migrations.Static;

public class InitStaticLanguagesDataSeeder : BaseAfterDeploymentDataSeeder
{
    private readonly ILanguageRepository _languageRepository;
    private readonly StaticLanguagesDataSeeder _languagesData;
    private readonly ITransactionBuilder _transactionBuilder;

    public InitStaticLanguagesDataSeeder(
        ILanguageRepository languageRepository,
        ITransactionBuilder transactionBuilder,
        StaticLanguagesDataSeeder languagesData,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
    {
        _languageRepository = languageRepository;
        _transactionBuilder = transactionBuilder;
        _languagesData = languagesData;
    }

    public override string Name => nameof(InitStaticLanguagesDataSeeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            foreach(var language in _languagesData.Languages)
            {
                _languageRepository.Add(language);
            }

            await transaction.Commit(cancellationToken);
        }
    }
}

public class StaticLanguagesDataSeeder
{
    public StaticLanguagesDataSeeder(List<Language> languages)
    {
        Languages = languages;
    }

    public List<Language> Languages
    {
        get; private set;
    }
}