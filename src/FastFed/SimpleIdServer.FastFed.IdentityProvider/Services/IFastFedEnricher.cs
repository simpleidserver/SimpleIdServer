// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace SimpleIdServer.FastFed.IdentityProvider.Services;

public interface IFastFedEnricher
{
    void EnrichFastFedRequest(Dictionary<string, object> dic);
}