// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.OAuth.Domains
{
    public class Token
    {
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string TokenType { get; set; }
        public string Data { get; set; }
        public string AuthorizationCode { get; set; }
        public DateTime? ExpirationTime { get; set; }
        public DateTime CreateDateTime { get; set; }
    }
}
