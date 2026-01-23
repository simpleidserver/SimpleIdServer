// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Template.Startup.Conf.Migrations.AfterDeployment;

public class ConfigureGotifyDataseeder : BaseAfterDeploymentDataSeeder
{
    private readonly ITransactionBuilder _transactionBuilder;
    private readonly IGotiySessionStore _gotiySessionStore;

    public ConfigureGotifyDataseeder(
        ITransactionBuilder transactionBuilder,
        IGotiySessionStore gotiySessionStore,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
    {
        _transactionBuilder = transactionBuilder;
        _gotiySessionStore = gotiySessionStore;
    }

    public override string Name => nameof(ConfigureGotifyDataseeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            foreach(var session in Sessions)
            {
                var existingSession = await _gotiySessionStore.GetByClientToken(session.ApplicationToken, cancellationToken);
                if(existingSession == null)
                {
                    continue;
                }

                _gotiySessionStore.Add(session);
            }

            await transaction.Commit(cancellationToken);
        }
    }

    private static List<GotifySession> Sessions = new List<GotifySession>
    {
        new GotifySession { ApplicationToken = "AvSdAw5ILVOdc7g", ClientToken = "CY2St_LANPO5L7P" },
        new GotifySession { ApplicationToken = "ADIeCkMigAnGLmq", ClientToken = "C9M4RGtX.OlYD1q" }
    };
}
