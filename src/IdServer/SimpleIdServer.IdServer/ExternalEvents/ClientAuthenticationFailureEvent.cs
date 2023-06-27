// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.ExternalEvents
{
    public class ClientAuthenticationFailureEvent : IExternalEvent
    {
        public string EventName => nameof(ClientAuthenticationFailureEvent);
        public string ClientId { get; set; }
        public string AuthMethod { get; set; }
        public string Realm { get; set; }
    }
}
