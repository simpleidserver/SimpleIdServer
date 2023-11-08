// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.ExternalEvents
{
    public class RemoveCertificateAuthoritySuccessEvent : IExternalEvent
    {
        public string EventName => nameof(RemoveCertificateAuthoritySuccessEvent);
        public string Realm { get; set; }
        public string Id { get; set; }
    }
}
