// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Website.Stores.ClientStore;

namespace SimpleIdServer.IdServer.Website.Stores.GroupStore
{
    [FeatureState]
    public record class GroupRolesState
    {
        public GroupRolesState()
        {

        }

        public GroupRolesState(IEnumerable<Scope> scopes, bool isLoading)
        {
            GroupRoles = scopes.Select(s => new SelectableGroupRole(s)).ToList();
            Count = GroupRoles.Count();
            IsLoading = isLoading;
        }

        public ICollection<SelectableGroupRole> GroupRoles { get; set; } = new List<SelectableGroupRole>();
        public bool IsLoading { get; set; } = false;
        public int Count { get; set; } = 0;
        public ICollection<EditableGroupScope> EditableGroupRoles { get; set; } = new List<EditableGroupScope>();
        public bool IsEditableRolesLoading { get; set; } = false;
        public int EditableGroupCount { get; set; } = 0;
    }

    public class SelectableGroupRole
    {
        public SelectableGroupRole(Scope value)
        {
            Value = value;
        }

        public bool IsNew { get; set; }
        public bool IsSelected { get; set; }
        public Scope Value { get; set; }
    }

    public class EditableGroupScope
    {
        public EditableGroupScope(Domains.Scope scope)
        {
            Value = scope;
        }

        public bool IsPresent { get; set; }
        public bool IsSelected { get; set; }
        public Domains.Scope Value { get; set; }
    }
}
