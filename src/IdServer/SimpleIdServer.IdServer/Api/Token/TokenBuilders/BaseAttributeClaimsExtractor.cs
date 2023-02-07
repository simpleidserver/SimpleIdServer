// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Api.Token.TokenBuilders
{
    public class BaseAttributeClaimsExtractor
    {
        public object Convert(string value, ScopeClaimMapper mapper)
        {
            if (mapper.TokenClaimJsonType == null) return null;
            switch(mapper.TokenClaimJsonType)
            {
                case TokenClaimJsonTypes.BOOLEAN:
                    return bool.Parse(value);
                case TokenClaimJsonTypes.STRING:
                    return value;
                case TokenClaimJsonTypes.LONG:
                    return long.Parse(value);
                case TokenClaimJsonTypes.INT:
                    return int.Parse(value);
                case TokenClaimJsonTypes.DATETIME:
                    var dt = DateTime.Parse(value);
                    return dt.ConvertToUnixTimestamp();
                default:
                    return JsonSerializer.Deserialize<Dictionary<string, object>>(value);
            }
        }
    }
}
