// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Domains.DTOs
{
    public static class JsonSerializerExtensions
    {
        public static ICollection<AuthorizationData> DeserializeAuthorizationDetails(string json) => DeserializeAuthorizationDetails(JsonNode.Parse(json));

        public static ICollection<AuthorizationData> DeserializeAuthorizationDetails(JsonNode jsonNode)
        {
            if (jsonNode == null) return new List<AuthorizationData>();
            if(jsonNode is JsonValue)
                return DeserializeAuthorizationDetails(JsonNode.Parse(jsonNode.GetValue<string>()));

            if (jsonNode is JsonObject)
            {
                var result = Deserialize(jsonNode.AsObject());
                return new List<AuthorizationData> { result };
            }
            else
            {
                var arr = jsonNode.AsArray();
                var result = new List<AuthorizationData>();
                foreach (var record in arr)
                {
                    result.Add(Deserialize(record.AsObject()));
                }

                return result;
            }

            AuthorizationData Deserialize(JsonObject jsonObj)
            {
                var result = new AuthorizationData();
                foreach (var record in jsonObj)
                {
                    switch (record.Key)
                    {
                        case AuthorizationDataParameters.Type:
                            result.Type = record.Value.GetValue<string>();
                            break;
                        case AuthorizationDataParameters.Identifier:
                            result.Identifier = record.Value.GetValue<string>();
                            break;
                        case AuthorizationDataParameters.Locations:
                            result.Locations = record.Value.AsArray().Select(v => v.GetValue<string>()).ToList();
                            break;
                        case AuthorizationDataParameters.Actions:
                            result.Actions = record.Value.AsArray().Select(v => v.GetValue<string>()).ToList();
                            break;
                        case AuthorizationDataParameters.DataTypes:
                            result.DataTypes = record.Value.AsArray().Select(v => v.GetValue<string>()).ToList();
                            break;
                        case AuthorizationDataParameters.Types:
                            result.Types = record.Value.AsArray().Select(v => v.GetValue<string>()).ToList();
                            break;
                        default:
                            var val = record.Value as JsonValue;
                            if (val != null) result.AdditionalData.Add(record.Key, record.Value.GetValue<string>());
                            else result.AdditionalData.Add(record.Key, record.Value.ToJsonString());
                            break;
                    }
                }

                return result;
            }
        }
    }
}
