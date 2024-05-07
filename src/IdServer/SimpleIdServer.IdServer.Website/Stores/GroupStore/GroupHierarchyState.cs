// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Api.Groups;

namespace SimpleIdServer.IdServer.Website.Stores.GroupStore
{
    [FeatureState]
    public record GroupHierarchyState
    {
        public GroupHierarchyState()
        {
            
        }

        public List<GetHierarchicalGroupResult> Result { get; set; }
        public bool IsLoading { get; set; } = true;
    }
}
