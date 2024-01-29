// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Helpers
{
    public class PreAuthCode
    {
        public string ClientId { get; set; }
        public string Code {  get; set; }
        public IEnumerable<string> Scopes { get; set; }
        public string TransactionCode { get; set; }
        public string UserId { get; set; }
    }
}
