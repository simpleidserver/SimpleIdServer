// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store.EF;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Startup.Conf.Migrations.BeforeDeployment;

public class ClientTypeDataSeeder : EfdataSeeder.DataSeeder<StoreDbContext, StoreDbContext>
{
    public ClientTypeDataSeeder(StoreDbContext dbContext) : base(dbContext)
    {

    }

    public override bool IsBeforeDeployment => true;
    protected override string Name => "ClientTypeDataSeeder";

    protected override async Task Up(CancellationToken cancellationToken)
    {
        var oldClients = await DbContext.Database.SqlQueryRaw<OldClient>("SELECT * FROM Clients").ToListAsync(cancellationToken);
        oldClients.ForEach(async c =>
        {
            if(string.IsNullOrWhiteSpace(c.ClientType))
            {
                return;
            }

            if (Enum.TryParse<ClientTypes>(c.ClientType, true, out var clientType))
            {
                var cl = await DbContext.Clients.SingleAsync(ec => ec.Id == c.Id, cancellationToken);
                cl.ClientType = clientType;
                DbContext.Entry(cl).State = EntityState.Modified;
            }
        });
    }

    private class OldClient
    {
        public string Id { get; set; }
        public string? ClientType { get; set; }
    }
}
