// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.Scim.Helpers
{
    public interface IResourceTypeResolver
    {
        List<ResourceTypeResolutionResult> ResolveAll();
        ResourceTypeResolutionResult ResolveByResourceType(string resourceType);
    }
}
