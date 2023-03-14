// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace SimpleIdServer.IdServer.ExternalEvents
{
    public class ConsentGrantedEvent
    {
        public string UserName { get; set; }
        public string ClientId { get; set; }
        public IEnumerable<string> Scopes { get; set; }
        public IEnumerable<string> Claims { get; set; }
        public string Realm { get; set; }
    }
}
