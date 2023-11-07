// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.ExternalEvents;

public class UpdateScopeResourcesSuccessEvent : IExternalEvent
{
    public string EventName => nameof(UpdateScopeResourcesSuccessEvent);
    public string Realm { get; set; }
    public string Name { get; set; }
    public List<string> Resources { get; set; }
}
