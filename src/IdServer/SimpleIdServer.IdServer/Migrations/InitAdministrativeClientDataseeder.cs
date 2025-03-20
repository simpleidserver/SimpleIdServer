// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Config;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Migrations;

public class InitAdministrativeClientDataseeder : BaseClientDataseeder
{
    private readonly ITransactionBuilder _transactionBuilder;
    private readonly IRealmRepository _realmRepository;
    private readonly List<string> _redirectUrls;
    private readonly List<string> _postLogoutUrls;
    private readonly string _backChannelLogoutUrl;
    private readonly List<Scope> _additionalScopes;

    public InitAdministrativeClientDataseeder(
        List<string> redirectUrls, 
        List<string> postLogoutUrls, 
        string backchannelLogoutUrl,
        List<Scope> additionalScopes,
        ITransactionBuilder transactionBuilder,
        IRealmRepository realmRepository,
        IScopeRepository scopeRepository,
        IClientRepository clientRepository,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(realmRepository, scopeRepository, clientRepository, dataSeederExecutionHistoryRepository)
    {
        _transactionBuilder = transactionBuilder;
        _redirectUrls = redirectUrls;
        _postLogoutUrls = postLogoutUrls;
        _backChannelLogoutUrl = backchannelLogoutUrl;
        _additionalScopes = additionalScopes;
    }

    public override string Name => nameof(InitAdministrativeClientDataseeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            var newClient = BuildAdministrativeClient(_redirectUrls.ToArray(), _postLogoutUrls, _backChannelLogoutUrl, _additionalScopes.ToArray());
            await TryAddClient(newClient);
            await transaction.Commit(cancellationToken);
        }
    }

    private static Client BuildAdministrativeClient(string[] redirectUrls, List<string> postLogoutUrls, string backChannelLogoutUrl, Scope[] additionalScopes)
    {
        var builder = ClientBuilder.BuildTraditionalWebsiteClient("SIDS-manager", "password", null, redirectUrls)
            .EnableClientGrantType()
            .SetRequestObjectEncryption()
            .AddAuthDataTypes("photo")
            .SetClientName("SimpleIdServer manager")
            .SetBackChannelLogoutUrl(backChannelLogoutUrl)
            .SetClientLogoUri("https://cdn.logo.com/hotlink-ok/logo-social.png")
            .AddScope(
                DefaultScopes.Role,
                DefaultScopes.OpenIdScope,
                DefaultScopes.Profile,
                DefaultScopes.Provisioning,
                DefaultScopes.Users,
                DefaultScopes.Workflows,
                DefaultScopes.Acrs,
                DefaultScopes.ConfigurationsScope,
                DefaultScopes.AuthenticationSchemeProviders,
                DefaultScopes.AuthenticationMethods,
                DefaultScopes.RegistrationWorkflows,
                DefaultScopes.ApiResources,
                DefaultScopes.Auditing,
                DefaultScopes.Scopes,
                DefaultScopes.CertificateAuthorities,
                DefaultScopes.Clients,
                DefaultScopes.Realms,
                DefaultScopes.Groups,
                DefaultScopes.WebsiteAdministratorRole,
                DefaultScopes.Forms,
                DefaultScopes.RecurringJobs);
        foreach (var postLogoutUrl in postLogoutUrls)
        {
            builder.AddPostLogoutUri(postLogoutUrl);
        }

        builder.AddScope(additionalScopes);
        return builder.Build();
    }
}
