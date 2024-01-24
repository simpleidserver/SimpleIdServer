// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.ExternalEvents;

namespace SimpleIdServer.IdServer.CredentialIssuer.ExternalEvents;

internal class CredentialTemplateAddedSuccessEvent : IExternalEvent
{
    public string EventName => nameof(CredentialTemplateAddedSuccessEvent);
    public string Realm { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }
    public string LogoUrl { get; set; }
    public string Type { get; set; }
}
