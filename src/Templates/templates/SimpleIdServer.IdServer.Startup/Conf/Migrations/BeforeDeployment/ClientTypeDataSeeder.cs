// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using DataSeeder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<StoreDbContext> _logger;

    public ClientTypeDataSeeder(
        StoreDbContext dbcontext,
        ILogger<StoreDbContext> logger,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
    {
        _dbcontext = dbcontext;
        _logger = logger;
    }
    public override string Name => "ClientTypeDataSeeder";

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        if(_dbcontext.Database.IsInMemory())
        {
            return;
        }

        try
        {
            string query = GetSelectClientsQuery(_dbcontext);
            var oldClients = await _dbcontext.Database.SqlQueryRaw<OldClient>(query).ToListAsync(cancellationToken);
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
            await _dbcontext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
    }

    private static string GetSelectClientsQuery(DbContext dbContext)
    {
        var db = dbContext.Database;

        if (db.IsSqlServer())
        {
            return "SELECT [Id], [ClientType] FROM [Clients]";
        }
        else if (db.IsMySql())
        {
            return "SELECT `Id`, `ClientType` FROM `Clients`";
        }
        else if (db.IsSqlite())
        {
            return "SELECT Id, ClientType FROM Clients";
        }
        else if (db.IsNpgsql())
        {
            return @"SELECT ""Id"", ""ClientType"" FROM ""Clients""";
        }
        else
        {
            throw new NotSupportedException("Unsupported database provider.");
        }
    }

    private class OldClient
    {
        public string Id { get; set; }
        public string? ClientType { get; set; }
    }
}