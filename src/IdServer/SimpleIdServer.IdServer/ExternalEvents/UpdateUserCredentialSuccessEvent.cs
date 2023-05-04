// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.ExternalEvents
{
    public class UpdateUserCredentialSuccessEvent
    {
        public string Realm { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
