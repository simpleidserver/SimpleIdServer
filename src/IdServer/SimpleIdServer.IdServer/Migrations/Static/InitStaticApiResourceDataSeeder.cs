// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Migrations.Static;

public class InitStaticApiResourceDataSeeder : BaseAfterDeploymentDataSeeder
{
    private readonly IApiResourceRepository _apiResourceRepository;
    private readonly ITransactionBuilder _transactionBuilder;
    private readonly StaticApiResourcesDataSeeder _apiResourcesData;

    public InitStaticApiResourceDataSeeder(
        IApiResourceRepository apiResourceRepository,
        ITransactionBuilder transactionBuilder,
        StaticApiResourcesDataSeeder apiResourcesData,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository
        ) : base(dataSeederExecutionHistoryRepository)
    {
        _apiResourceRepository = apiResourceRepository;
        _transactionBuilder = transactionBuilder;
        _apiResourcesData = apiResourcesData;
    }

    public override string Name => nameof(InitStaticApiResourceDataSeeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            foreach (var apiResource in _apiResourcesData.ApiResources)
            {
                _apiResourceRepository.Add(apiResource);
            }

            await transaction.Commit(cancellationToken);
        }
    }
}
public class StaticApiResourcesDataSeeder
{
    public StaticApiResourcesDataSeeder(List<ApiResource> apiResources)
    {
        ApiResources = apiResources;
    }

    public List<ApiResource> ApiResources
    {
        get; private set;
    }
}