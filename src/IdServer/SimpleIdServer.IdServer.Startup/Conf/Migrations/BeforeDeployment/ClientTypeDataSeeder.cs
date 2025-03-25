// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using DataSeeder;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store.EF;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Startup.Conf.Migrations.BeforeDeployment;

public class ClientTypeDataSeeder : BaseBeforeDeploymentDataSeeder
{
    private readonly StoreDbContext _dbcontext;

    public ClientTypeDataSeeder(StoreDbContext dbcontext, IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
    {
        _dbcontext = dbcontext;
    }
    public override string Name => "ClientTypeDataSeeder";

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        var oldClients = await _dbcontext.Database.SqlQueryRaw<OldClient>("SELECT * FROM Clients").ToListAsync(cancellationToken);
        oldClients.ForEach(c =>
        {
            if (string.IsNullOrWhiteSpace(c.ClientType))
            {
                return;
            }

            if (Enum.TryParse<ClientTypes>(c.ClientType, true, out var clientType))
            {
                var client = new Client
                {
                    Id = c.Id
                };
                _dbcontext.Attach(client);
                client.ClientType = clientType;
            }
        });
    }

    private class OldClient
    {
        public string Id { get; set; }
        public string? ClientType { get; set; }
    }
}