// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Migrations.Static;

public class InitStaticUsersDataSeeder : BaseAfterDeploymentDataSeeder
{
    private readonly StaticUsersDataSeeder _staticUsersData;
    private readonly ITransactionBuilder _transactionBuilder;
    private readonly IUserRepository _userRepository;

    public InitStaticUsersDataSeeder(
        IUserRepository userRepository,
        StaticUsersDataSeeder staticUsersData, 
        ITransactionBuilder transactionBuilder,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
    {
        _userRepository = userRepository;
        _staticUsersData = staticUsersData;
        _transactionBuilder = transactionBuilder;
    }

    public override string Name => nameof(InitStaticUsersDataSeeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using(var transaction = _transactionBuilder.Build())
        {
            foreach (var user in _staticUsersData.Users)
            {
                _userRepository.Add(user);
            }

            await transaction.Commit(cancellationToken);
        }
    }
}

public class StaticUsersDataSeeder
{
    public StaticUsersDataSeeder(List<User> users)
    {
        Users = users;
    }

    public List<User> Users
    {
        get; private set;
    }
}