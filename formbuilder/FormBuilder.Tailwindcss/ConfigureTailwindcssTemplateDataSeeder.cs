﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using FormBuilder.Stores;

namespace FormBuilder.Tailwindcss;

public class ConfigureTailwindcssTemplateDataSeeder : BaseAfterDeploymentDataSeeder
{
    private readonly ITemplateStore _templateStore;

    public ConfigureTailwindcssTemplateDataSeeder(ITemplateStore templateStore, IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
    {
        _templateStore = templateStore;
    }

    public override string Name => nameof(ConfigureTailwindcssTemplateDataSeeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        var existingTemplates = await _templateStore.GetByName(TailwindCssTemplate.Name, cancellationToken);
        if (existingTemplates.Any())
        {
            return;
        }

        _templateStore.Add(TailwindCssTemplate.DefaultTemplate);
        await _templateStore.SaveChanges(cancellationToken);
    }
}
