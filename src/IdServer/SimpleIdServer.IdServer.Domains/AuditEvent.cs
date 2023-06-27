// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Domains
{
    public class AuditEvent
    {
        public string Id { get; set; } = null!;
        public string EventName { get; set; } = null!;
        public string Realm { get; set; } = null!;
        public bool IsError { get; set; }
        public string Description { get; set; } = null!;
        public string? ErrorMessage { get; set; } = null;
        public DateTime CreateDateTime { get; set; }
        public string? ClientId { get; set; } = null;
        public string? UserName { get; set; } = null;
        public string? RequestJSON { get; set; } = null;
        public string? RedirectUrl { get; set; } = null;
        public string? AuthMethod { get; set; } = null;
        public string[] Scopes { get; set; } = new string[0];
        public string[] Claims { get; set; } = new string[0];
    }
}
