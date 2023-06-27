// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.ExternalEvents
{
    public class ClientAuthenticationSuccessEvent : IExternalEvent
    {
        public string EventName => nameof(ClientAuthenticationSuccessEvent);
        public string ClientId { get; set; }
        public string AuthMethod { get; set; }
        public string Realm { get; set; }
    }
}
