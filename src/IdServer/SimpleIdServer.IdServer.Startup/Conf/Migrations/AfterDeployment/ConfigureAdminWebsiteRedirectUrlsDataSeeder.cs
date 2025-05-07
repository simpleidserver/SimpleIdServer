// Copyright (c) SimpleIdServer. AllClients rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using SimpleIdServer.IdServer.Config;
using SimpleIdServer.IdServer.Stores;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Startup.Conf.Migrations.AfterDeployment;

public class ConfigureAdminWebsiteRedirectUrlsDataSeeder : BaseAfterDeploymentDataSeeder
{
    private readonly IClientRepository _clientRepository;
    private readonly ITransactionBuilder _transactionBuilder;
    private readonly IdentityServerConfiguration _configuration;

    public ConfigureAdminWebsiteRedirectUrlsDataSeeder(
        IClientRepository clientRepository,
        ITransactionBuilder transactionBuilder,
        IdentityServerConfiguration configuration,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
    {
        _clientRepository = clientRepository;
        _transactionBuilder = transactionBuilder;
        _configuration = configuration;
    }

    public override string Name => nameof(ConfigureAdminWebsiteRedirectUrlsDataSeeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            var adminUriUrl = _configuration.AdminUiUrl ?? "https://localhost:5002";
            var adminClient = await _clientRepository.GetByClientId(Constants.DefaultRealm, DefaultClients.SidAdminClientId, cancellationToken);
            var redirectionUrls = adminClient.RedirectionUrls.Where(r => !string.IsNullOrWhiteSpace(r)).ToList();
            var postlogoutRedirectionUrls = adminClient.PostLogoutRedirectUris.Where(r => !string.IsNullOrWhiteSpace(r)).ToList();
            var redirectUrl = $"{adminUriUrl}/*";
            if (!redirectionUrls.Contains(redirectUrl))
            {
                redirectionUrls.Add(redirectUrl);
            }

            var postLogoutRedirectUrl = $"{adminUriUrl}/signout-callback-oidc";
            if (!postlogoutRedirectionUrls.Contains(postLogoutRedirectUrl))
            {
                postlogoutRedirectionUrls.Add(postLogoutRedirectUrl);
            }

            adminClient.PostLogoutRedirectUris = postlogoutRedirectionUrls;
            adminClient.RedirectionUrls = redirectionUrls;
            adminClient.BackChannelLogoutUri = $"{adminUriUrl}/bc-logout";
            await transaction.Commit(cancellationToken);
        }
    }
}
