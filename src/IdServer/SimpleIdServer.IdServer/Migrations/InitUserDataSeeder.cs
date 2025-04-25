// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using DataSeeder;
using SimpleIdServer.IdServer.Config;
using SimpleIdServer.IdServer.Stores;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Migrations;

public class InitUserDataSeeder : BaseAfterDeploymentDataSeeder
{
    private readonly IUserRepository _userRepository;
    private readonly ITransactionBuilder _transactionBuilder;
    private readonly IGroupRepository _groupRepository;

    public InitUserDataSeeder(
        IRealmRepository realmRepository,
        IUserRepository userRepository,
        IGroupRepository groupRepository,
        ITransactionBuilder transactionBuilder,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
    {
        _userRepository = userRepository;
        _groupRepository = groupRepository;
        _transactionBuilder = transactionBuilder;
    }

    public override string Name => nameof(InitUserDataSeeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            var subjects = DefaultUsers.All.Select(u => u.Name).ToList();
            var existingUsers = await _userRepository.GetUsersBySubjects(subjects, Constants.DefaultRealm, cancellationToken);
            var unknownUsers = DefaultUsers.All.Where(u => !existingUsers.Any(eu => eu.Name == u.Name));
            foreach (var unknownUser in unknownUsers)
            {
                _userRepository.Add(unknownUser);
            }

            await transaction.Commit(cancellationToken);
        }
    }
}
