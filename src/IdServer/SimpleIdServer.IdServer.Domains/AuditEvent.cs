// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains
{
    public class AuditEvent
    {
        [JsonPropertyName(AuditingEventNames.Id)]
        public string Id { get; set; } = null!;
        [JsonPropertyName(AuditingEventNames.EventName)]
        public string EventName { get; set; } = null!;
        [JsonPropertyName(AuditingEventNames.Realm)]
        public string Realm { get; set; } = null!;
        [JsonPropertyName(AuditingEventNames.IsError)]
        public bool IsError { get; set; }
        [JsonPropertyName(AuditingEventNames.Description)]
        public string Description { get; set; } = null!;
        [JsonPropertyName(AuditingEventNames.ErrorMessage)]
        public string? ErrorMessage { get; set; } = null;
        [JsonPropertyName(AuditingEventNames.CreateDateTime)]
        public DateTime CreateDateTime { get; set; }
        [JsonPropertyName(AuditingEventNames.ClientId)]
        public string? ClientId { get; set; } = null;
        [JsonPropertyName(AuditingEventNames.UserName)]
        public string? UserName { get; set; } = null;
        [JsonPropertyName(AuditingEventNames.RequestJSON)]
        public string? RequestJSON { get; set; } = null;
        [JsonPropertyName(AuditingEventNames.RedirectUrl)]
        public string? RedirectUrl { get; set; } = null;
        [JsonPropertyName(AuditingEventNames.AuthMethod)]
        public string? AuthMethod { get; set; } = null;
        [JsonPropertyName(AuditingEventNames.Scopes)]
        public string[] Scopes { get; set; } = new string[0];
        [JsonPropertyName(AuditingEventNames.Claims)]
        public string[] Claims { get; set; } = new string[0];
    }
}
