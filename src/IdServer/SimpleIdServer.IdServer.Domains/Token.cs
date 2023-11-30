// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Domains
{
    public class Token
    {
        public string Id { get; set; } = null!;
        public string? SessionId { get; set; } = null;
        public string ClientId { get; set; } = null!;
        public string TokenType { get; set; } = null!;
        public AccessTokenTypes? AccessTokenType { get; set; } = null;
        public string? Data { get; set; } = null;
        public string? OriginalData { get; set; } = null;
        public string? AuthorizationCode { get; set; } = null;
        public string? GrantId { get; set; } = null;
        public string? Jkt { get; set; } = null;
        public DateTime? ExpirationTime { get; set; }
        public DateTime CreateDateTime { get; set; }
        public bool IsExpired()
        {
            return DateTime.UtcNow > ExpirationTime;
        }
    }
}
