// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.GroupStore
{
    [FeatureState]
    public record GroupState
    {
        public GroupState() { }

        public GroupState(bool isLoading, Group? group, Group? rootGroup)
        {
            IsLoading = isLoading;
            Group = group;
            RootGroup = rootGroup;
        }


        public bool IsLoading { get; set; } = true;
        public Group? Group { get; set; } = null;
        public Group? RootGroup { get; set; } = null;
    }
}
