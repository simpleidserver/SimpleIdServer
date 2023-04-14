// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.UserStore
{
    [FeatureState]
    public record UserGroupsState
    {
        public UserGroupsState() { }

        public UserGroupsState(IEnumerable<Group> groups, bool isLoading)
        {
            Groups = groups.Select(g => new SelectableUserGroup(g));
            Count = groups.Count();
            IsLoading = isLoading;
        }

        public IEnumerable<SelectableUserGroup> Groups { get; set; } = new List<SelectableUserGroup>();
        public int Count { get; set; } = 0;
        public bool IsLoading { get; set; } = false;
        public IEnumerable<EditableUserGroup> EditableGroups { get; set; } = new List<EditableUserGroup>();
        public int EditableGroupsCount { get; set; } = 0;
        public bool IsEditableGroupsLoading { get; set; } = false;
    }

    public class SelectableUserGroup
    {
        public SelectableUserGroup(Group value)
        {
            Value = value;
        }

        public Group Value { get; set; }
        public bool IsNew { get; set; }
        public bool IsSelected { get; set; }
    }

    public class EditableUserGroup
    {
        public EditableUserGroup(Group value)
        {
            Value = value;
        }

        public bool IsPresent { get; set; }
        public bool IsSelected { get; set; }
        public Group Value { get; set; }
    }
}
