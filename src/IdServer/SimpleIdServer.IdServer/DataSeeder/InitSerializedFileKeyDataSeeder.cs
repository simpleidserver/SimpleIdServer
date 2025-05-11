// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using DataSeeder;
using SimpleIdServer.IdServer.Config;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.DataSeeder;

public class InitSerializedFileKeyDataSeeder : BaseAfterDeploymentDataSeeder
{
    private readonly IFileSerializedKeyStore _fileSerializedKeyStore;
    private readonly IRealmRepository _realmRepository;
    private readonly ITransactionBuilder _transactionBuilder;

    public InitSerializedFileKeyDataSeeder(
        IFileSerializedKeyStore fileSerializedKeyStore,
        IRealmRepository realmRepository,
        ITransactionBuilder transactionBuilder,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
    {
        _fileSerializedKeyStore = fileSerializedKeyStore;
        _realmRepository = realmRepository;
        _transactionBuilder = transactionBuilder;
    }

    public override string Name => nameof(InitSerializedFileKeyDataSeeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            var keyIds = DefaultKeys.All.Select(k => k.KeyId).ToList();
            var masterRealm = await _realmRepository.Get(Constants.DefaultRealm, cancellationToken);
            var existingSerializedFileKeys = _fileSerializedKeyStore.GetByKeyIds(keyIds, cancellationToken);
            var unknownSerializedFileKeys = DefaultKeys.All.Where(k => !existingSerializedFileKeys.Result.Any(e => e.KeyId == k.KeyId)).ToList();
            foreach (var unknownSerializedFileKey in unknownSerializedFileKeys)
            {
                unknownSerializedFileKey.Realms = new List<Realm>
                {
                    masterRealm
                };
                _fileSerializedKeyStore.Add(unknownSerializedFileKey);
            }

            await transaction.Commit(cancellationToken);
        }
    }
}
