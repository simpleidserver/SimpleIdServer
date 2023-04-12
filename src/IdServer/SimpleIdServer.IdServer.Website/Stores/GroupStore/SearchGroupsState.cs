// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.GroupStore
{
    [FeatureState]
    public record SearchGroupsState
    {
        public SearchGroupsState() { }

        public SearchGroupsState(bool isLoading, IEnumerable<Group> groups, int nbGroups)
        {
            Groups = groups.Select(g => new SelectableGroup(g));
            IsLoading = isLoading;
            Count = nbGroups;
        }

        public IEnumerable<SelectableGroup>? Groups { get; set; } = null;
        public int Count { get; set; } = 0;
        public bool IsLoading { get; set; } = true;
    }

    public class SelectableGroup
    {
        public SelectableGroup(Group group)
        {
            Group = group;
        }

        public bool IsSelected { get; set; } = false;
        public bool IsNew { get; set; } = false;
        public Group Group { get; set; }
    }
}
