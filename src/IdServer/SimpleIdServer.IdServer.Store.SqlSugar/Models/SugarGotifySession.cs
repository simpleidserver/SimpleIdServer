// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models
{
    [SugarTable("GotifySessions")]
    public class SugarGotifySession
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string ApplicationToken { get; set; } = null!;
        public string ClientToken { get; set; } = null!;

        public GotifySession ToDomain()
        {
            return new GotifySession
            {
                ApplicationToken = ApplicationToken,
                ClientToken = ClientToken,
            };
        }
    }
}
