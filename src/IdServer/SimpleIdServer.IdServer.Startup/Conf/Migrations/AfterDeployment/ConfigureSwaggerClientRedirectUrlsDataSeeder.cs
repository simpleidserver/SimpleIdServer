// Copyright (c) SimpleIdServer. AllClients rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using SimpleIdServer.IdServer.Stores;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Startup.Conf.Migrations.AfterDeployment;

public class ConfigureSwaggerClientRedirectUrlsDataSeeder : BaseAfterDeploymentDataSeeder
{
    private readonly IClientRepository _clientRepository;
    private readonly ITransactionBuilder _transactionBuilder;
    private readonly IdentityServerConfiguration _configuration;

    public ConfigureSwaggerClientRedirectUrlsDataSeeder(
        IClientRepository clientRepository,
        ITransactionBuilder transactionBuilder,
        IdentityServerConfiguration configuration,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
    {
        _clientRepository = clientRepository;
        _transactionBuilder = transactionBuilder;
        _configuration = configuration;
    }

    public override string Name => nameof(ConfigureSwaggerClientRedirectUrlsDataSeeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            var idserverUrl = _configuration.Authority ?? "https://localhost:5001";
            var swaggerClient = await _clientRepository.GetByClientId(Constants.DefaultRealm, "swaggerClient", cancellationToken);
            var urls = new[]
            {
                $"{idserverUrl}/swagger/oauth2-redirect.html",
                $"{idserverUrl}/(.*)/swagger/oauth2-redirect.html"
            };
            var redirectionUrls = swaggerClient.RedirectionUrls.Where(r => !string.IsNullOrWhiteSpace(r)).ToList();
            foreach (var url in urls)
            {
                if (!redirectionUrls.Contains(url))
                {
                    redirectionUrls.Add(url);
                }
            }

            swaggerClient.RedirectionUrls = redirectionUrls;
            await transaction.Commit(cancellationToken);
        }
    }
}
