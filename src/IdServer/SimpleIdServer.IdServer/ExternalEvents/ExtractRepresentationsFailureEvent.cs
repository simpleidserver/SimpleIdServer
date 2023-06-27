// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.ExternalEvents
{
    public class ExtractRepresentationsFailureEvent : IExternalEvent
    {
        public string EventName => nameof(ExtractRepresentationsFailureEvent);
        public string IdentityProvisioningName { get; set; }
        public string ErrorMessage { get; set; }
        public string Realm { get; set; }
    }
}
