using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Extractors
{
    public class BaseClaimExtractor
    {
        public object Convert(string value, IClaimMappingRule mapper)
        {
            if (mapper.TokenClaimJsonType == null) return null;
            switch (mapper.TokenClaimJsonType)
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
