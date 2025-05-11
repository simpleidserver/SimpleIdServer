// Copyright (c) SimpleIdServer. AllClients rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.DataSeeder;

public class InitCertificateAuthoritiesDataseeder : BaseAfterDeploymentDataSeeder
{
    private readonly ITransactionBuilder _transactionBuilder;
    private readonly ICertificateAuthorityRepository _certificateAuthorityRepository;
    private readonly IRealmRepository _realmRepository;

    public InitCertificateAuthoritiesDataseeder(
        ITransactionBuilder transactionBuilder,
        ICertificateAuthorityRepository certificateAuthorityRepository,
        IRealmRepository realmRepository,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
    {
        _transactionBuilder = transactionBuilder;
        _certificateAuthorityRepository = certificateAuthorityRepository;
        _realmRepository = realmRepository;
    }

    public override string Name => nameof(InitCertificateAuthoritiesDataseeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            var rootCa = RootCa;
            var masterRealm = await _realmRepository.Get(Constants.DefaultRealm, cancellationToken);
            var existingCa = await _certificateAuthorityRepository.Get(Constants.DefaultRealm, rootCa.SubjectName, cancellationToken);
            if(existingCa != null)
            {
                existingCa.Realms = new List<Realm>
                {
                    masterRealm
                };
                _certificateAuthorityRepository.Add(existingCa);
            }

            await transaction.Commit(cancellationToken);
        }
    }

    private static CertificateAuthority RootCa => CertificateAuthorityBuilder.Create("CN=simpleIdServerCA", Config.DefaultRealms.Master).Build();
}
