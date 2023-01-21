// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Domains
{
    public class BCAuthorizeHistory
    {
        public DateTime StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string? Message { get; set; } = null;
        public BCAuthorizeStatus Status { get; set; }
    }
}
