// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains.DTOs;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains
{
    public class ApiResource
    {
        [JsonPropertyName(ApiResourceNames.Id)]
        public string Id { get; set; }
        /// <summary>
        /// URL : Target service or resource to which access is being requested.
        /// </summary>
        [JsonPropertyName(ApiResourceNames.Name)]
        public string Name { get; set; } = null!;
        /// <summary>
        /// Client identifier.
        /// </summary>
        [JsonPropertyName(ApiResourceNames.Audience)]
        public string? Audience { get; set; } = null;
        [JsonPropertyName(ApiResourceNames.Description)]
        public string? Description { get; set; } = null;
        [JsonPropertyName(ApiResourceNames.CreateDatetime)]
        public DateTime CreateDateTime { get; set; }
        [JsonPropertyName(ApiResourceNames.UpdateDatetime)]
        public DateTime UpdateDateTime { get; set; }
        [JsonPropertyName(ApiResourceNames.Scopes)]
        public ICollection<Scope> Scopes { get; set; } = new List<Scope>();
        [JsonIgnore]
        public ICollection<Realm> Realms { get; set; } = new List<Realm>();
    }
}
