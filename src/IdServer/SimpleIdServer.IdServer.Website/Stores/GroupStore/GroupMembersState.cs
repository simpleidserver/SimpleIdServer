// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.GroupStore
{
    [FeatureState]
    public record GroupMembersState
    {
        public GroupMembersState()
        {

        }

        public GroupMembersState(IEnumerable<Group> members, bool isLoading)
        {
            Members = members.Select(m => new SelectableGroupMember(m)).ToList();
            Count = members.Count();
            IsLoading = isLoading;
        }

        public ICollection<SelectableGroupMember> Members { get; set; } = new List<SelectableGroupMember>();
        public int Count { get; set; }
        public bool IsLoading { get; set; }
    }

    public class SelectableGroupMember
    {
        public SelectableGroupMember(Group value)
        {
            Value = value;
        } 

        public Group Value { get; set; }
        public bool IsSelected { get; set; }
        public bool IsNew { get; set; }
    }
}
