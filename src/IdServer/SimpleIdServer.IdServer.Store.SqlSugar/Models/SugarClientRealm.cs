// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models
{
    [SugarTable("ClientRealm")]
    public class SugarClientRealm
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string ClientsId { get; set; } = null!;
        [SugarColumn(IsPrimaryKey = true)]
        public string RealmsName { get; set; } = null!;
    }
}
