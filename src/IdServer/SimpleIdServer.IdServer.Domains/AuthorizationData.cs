// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains.DTOs;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains
{
    [JsonConverter(typeof(AuthorizationDataConverter))]
    public class AuthorizationData
    {
        /// <summary>
        /// Types of authorization data.
        /// </summary>
        [JsonPropertyName(AuthorizationDataParameters.Type)]
        public string Type { get; set; } = null!;
        /// <summary>
        /// An array of strings representing the location of the resource or resource server.
        /// This is typically composed of URIs.
        /// </summary>
        [JsonPropertyName(AuthorizationDataParameters.Locations)]
        public ICollection<string> Locations { get; set; } = new List<string>();
        /// <summary>
        /// An array of strings representing the kinds of actions to be taken at the resource.
        /// The values of the strings are determined by the API being protected.
        /// </summary>
        [JsonPropertyName(AuthorizationDataParameters.Actions)]
        public ICollection<string> Actions { get; set; } = new List<string>();
        /// <summary>
        /// An array of strings representing the kinds of data being requested from the resource.
        /// </summary>
        [JsonPropertyName(AuthorizationDataParameters.DataTypes)]
        public ICollection<string> DataTypes { get; set; } = new List<string>();
        /// <summary>
        /// A string identifier indicating a specific resource available at the API.
        /// </summary>
        [JsonPropertyName(AuthorizationDataParameters.Identifier)]
        public string? Identifier { get; set; } = null;
        [JsonIgnore]
        public Dictionary<string, string> AdditionalData { get; set; } = new Dictionary<string, string>();

        public Dictionary<string, object> Serialize()
        {
            var result = new Dictionary<string, object>
            {
                { AuthorizationDataParameters.Type, Type }
            };
            if (Locations != null && Locations.Any())
                result.Add(AuthorizationDataParameters.Locations, Locations);

            if (Actions != null && Actions.Any())
                result.Add(AuthorizationDataParameters.Actions, Actions);

            if (DataTypes != null && DataTypes.Any())
                result.Add(AuthorizationDataParameters.DataTypes, DataTypes);

            if (!string.IsNullOrWhiteSpace(Identifier))
                result.Add(AuthorizationDataParameters.Identifier, Identifier);

            if (AdditionalData != null)
                foreach (var kvp in AdditionalData)
                    result.Add(kvp.Key, SerializeJson(JsonNode.Parse(kvp.Value)));

            return result;
        }

        private object SerializeJson(JsonNode jsonNode)
        {
            if (jsonNode is JsonValue)
                return jsonNode.GetValue<string>();

            if (jsonNode is JsonObject)
            {
                var jsonObj = jsonNode as JsonObject;
                var dic = new Dictionary<string, object>();
                foreach (var key in jsonObj)
                {
                    dic.Add(key.Key, SerializeJson(key.Value));
                }

                return dic;
            }

            var jsonArr = jsonNode as JsonArray;
            var result = new List<object>();
            foreach (var record in jsonArr)
                result.Add(SerializeJson(record));
            return result;
        }
    }
}
