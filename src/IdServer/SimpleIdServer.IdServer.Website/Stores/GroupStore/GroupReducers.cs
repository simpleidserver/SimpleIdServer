// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Website.Stores.ScopeStore;

namespace SimpleIdServer.IdServer.Website.Stores.GroupStore
{
    public class GroupReducers
    {
        #region SearchGroupsState

        [ReducerMethod]
        public static SearchGroupsState ReduceSearchGroupsAction(SearchGroupsState state, SearchGroupsAction act) => new(isLoading: true, groups: new List<Group>(), 0);

        [ReducerMethod]
        public static SearchGroupsState ReduceSearchGroupsSuccessAction(SearchGroupsState state, SearchGroupsSuccessAction act)
        {
            return state with
            {
                IsLoading = false,
                Count = act.Count,
                Groups = act.Groups.Select(g => new SelectableGroup(g))
            };
        }

        [ReducerMethod]
        public static SearchGroupsState ReduceAddGroupAction(SearchGroupsState state, AddGroupAction act)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static SearchGroupsState ReduceAddGroupFailureAction(SearchGroupsState state, AddGroupFailureAction act)
        {
            return state with
            {
                IsLoading = false
            };
        }

        [ReducerMethod]
        public static SearchGroupsState ReduceAddGroupSuccessAction(SearchGroupsState state, AddGroupSuccessAction act)
        {
            if (!string.IsNullOrWhiteSpace(act.ParentGroupId)) return state with
            {
                IsLoading = false
            };
            var groups = state.Groups.ToList();
            groups.Add(new SelectableGroup(new Group { 
                CreateDateTime = DateTime.Now, 
                UpdateDateTime = DateTime.Now, 
                Id = act.Id, 
                Name = act.Name, 
                Description = act.Description, 
                FullPath = act.FullPath })
            {
                IsNew = true
            });
            return state with
            {
                Groups = groups,
                Count = groups.Count,
                IsLoading = false
            };
        }

        [ReducerMethod]
        public static SearchGroupsState ReduceRemoveSelectedGroupsAction(SearchGroupsState state, RemoveSelectedGroupsAction act)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static SearchGroupsState ReduceRemoveSelectedGroupsSuccessAction(SearchGroupsState state, RemoveSelectedGroupsSuccessAction act)
        {
            if (state.Groups == null) return state with
            {
                IsLoading = false
            };
            var groups = state.Groups.ToList();
            groups = groups.Where(g => !act.FullPathLst.Contains(g.Group.FullPath)).ToList();
            return state with
            {
                Groups = groups,
                Count = groups.Count,
                IsLoading = false
            };
        }

        [ReducerMethod]
        public static SearchGroupsState ReduceToggleAllGroupSelectionAction(SearchGroupsState state, ToggleAllGroupSelectionAction act)
        {
            var groups = state.Groups.ToList();
            foreach (var grp in groups)
                grp.IsSelected = act.IsSelected;
            return state with
            {
                Groups = groups
            };
        }

        [ReducerMethod]
        public static SearchGroupsState ReduceToggleGroupSelectionAction(SearchGroupsState state, ToggleGroupSelectionAction act)
        {
            var groups = state.Groups.ToList();
            var grp = groups.Single(g => g.Group.Id == act.GroupId);
            grp.IsSelected = act.IsSelected;
            return state with
            {
                Groups = groups,
            };
        }

        #endregion

        #region UpdateGroupState

        [ReducerMethod]
        public static UpdateGroupState ReduceAddGroupAction(UpdateGroupState state, AddGroupAction act) => new UpdateGroupState { ErrorMessage = null, IsUpdating = true };

        [ReducerMethod]
        public static UpdateGroupState ReduceAddGroupSuccessAction(UpdateGroupState state, AddGroupSuccessAction act) => new UpdateGroupState { ErrorMessage = null, IsUpdating = false };

        [ReducerMethod]
        public static UpdateGroupState ReduceAddGroupFailureAction(UpdateGroupState state, AddGroupFailureAction act) => new UpdateGroupState { ErrorMessage = act.ErrorMessage, IsUpdating = false };

        [ReducerMethod]
        public static UpdateGroupState ReduceAddGroupRolesAction(UpdateGroupState state, AddGroupRolesAction act) => new UpdateGroupState { ErrorMessage = null, IsUpdating = true };

        [ReducerMethod]
        public static UpdateGroupState ReduceAddGroupRolesSuccessAction(UpdateGroupState state, AddGroupRolesSuccessAction act) => new UpdateGroupState { ErrorMessage = null, IsUpdating = false };

        [ReducerMethod]
        public static UpdateGroupState ReduceStartAddGroupAction(UpdateGroupState state, StartAddGroupAction act) => new UpdateGroupState { ErrorMessage = null, IsUpdating = false  };

        #endregion

        #region GroupState

        [ReducerMethod]
        public static GroupState ReduceGetGroupAction(GroupState state, GetGroupAction act) => new(true, null, null);

        [ReducerMethod]
        public static GroupState ReduceGetGroupSuccessAction(GroupState state, GetGroupSuccessAction act) => new(false, act.Group, act.RootGroup);

        [ReducerMethod]
        public static GroupState ReduceGetGroupFailureAction(GroupState state, GetGroupFailureAction act) => new(false, null, null);

        [ReducerMethod]
        public static GroupState ReduceAddGroupSuccessAction(GroupState state, AddGroupSuccessAction act)
        {
            if (string.IsNullOrWhiteSpace(act.ParentGroupId) || state.Group.Id != act.ParentGroupId) return state;
            var children = state.Group.Children.ToList();
            children.Add(new Group
            {
                Id = act.Id,
                Name = act.Name,
                Description= act.Description
            });
            var group = state.Group;
            group.Children = children;
            return state with
            {
                Group = group
            };
        }

        [ReducerMethod]
        public static GroupState ReduceRemoveSelectedGroupsSuccessAction(GroupState state, RemoveSelectedGroupsSuccessAction act)
        {
            if (state.Group == null) return state;
            var group = state.Group;
            if(group.Children != null)
            {
                var children = group.Children.ToList();
                children = children.Where(c => !act.FullPathLst.Contains(c.FullPath)).ToList();
                group.Children = children;
            }

            return state with
            {
                Group = group
            };
        }

        #endregion

        #region GroupMembersState

        [ReducerMethod]
        public static GroupMembersState ReduceGetGroupAction(GroupMembersState state, GetGroupAction act) => new(new List<Group>(), false);

        [ReducerMethod]
        public static GroupMembersState ReduceGetGroupSuccessAction(GroupMembersState state, GetGroupSuccessAction act)
        {
            return state with
            {
                IsLoading = false,
                Count = act.Group.Children.Count(),
                Members = act.Group.Children.Select(c => new SelectableGroupMember(c)).ToList()
            };
        }

        [ReducerMethod]
        public static GroupMembersState ReduceSelectAllGroupMembersAction(GroupMembersState state, SelectAllGroupMembersAction action)
        {
            var members = state.Members.ToList();
            foreach (var member in members)
                member.IsSelected = action.IsSelected;

            return state with
            {
                Members = members
            };
        }

        [ReducerMethod]
        public static GroupMembersState ReduceSelectGroupMemberAction(GroupMembersState state, SelectGroupMemberAction action)
        {
            var members = state.Members.ToList();
            var member = members.Single(m => m.Value.Id == action.MemberId);
            member.IsSelected = action.IsSelected;
            return state with
            {
                Members = members
            };
        }

        [ReducerMethod]
        public static GroupMembersState ReduceAddGroupAction(GroupMembersState state, AddGroupAction action)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static GroupMembersState ReduceAddGroupFailureAction(GroupMembersState state, AddGroupFailureAction action)
        {
            return state with
            {
                IsLoading = false
            };
        }

        [ReducerMethod]
        public static GroupMembersState ReduceAddGroupSuccessAction(GroupMembersState state, AddGroupSuccessAction action)
        {
            if (string.IsNullOrWhiteSpace(action.ParentGroupId)) return state with
            {
                IsLoading = false
            };
            var members = state.Members.ToList();
            members.Add(new SelectableGroupMember(new Group { Id = action.Id, Name = action.Name, ParentGroupId = action.ParentGroupId, Description = action.Description, FullPath = action.FullPath, CreateDateTime = DateTime.Now, UpdateDateTime = DateTime.Now })
            {
                IsNew = true
            });
            return state with
            {
                Members = members,
                Count = members.Count,
                IsLoading = false
            };
        }

        [ReducerMethod]
        public static GroupMembersState ReduceRemoveSelectedGroupMembersAction(GroupMembersState state, RemoveSelectedGroupsAction action)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static GroupMembersState ReduceRemoveSelectedGroupMembersSuccessAction(GroupMembersState state, RemoveSelectedGroupsSuccessAction action)
        {
            var members = state.Members.ToList();
            members = members.Where(m => !action.FullPathLst.Any(f => m.Value.FullPath.StartsWith(f))).ToList();
            return state with
            {
                IsLoading = false,
                Count = members.Count(),
                Members = members
            };
        }

        #endregion

        #region GroupRolesState

        [ReducerMethod]
        public static GroupRolesState ReduceGetGroupAction(GroupRolesState state, GetGroupAction act) => new(new List<Scope>(), true);

        [ReducerMethod]
        public static GroupRolesState ReduceGetGroupSuccessAction(GroupRolesState state, GetGroupSuccessAction act) => new(act.Group.Roles, false);

        [ReducerMethod]
        public static GroupRolesState ReduceRemoveSelectedGroupRolesAction(GroupRolesState state, RemoveSelectedGroupRolesAction act)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static GroupRolesState ReduceRemoveSelectedGroupRolesSuccessAction(GroupRolesState state, RemoveSelectedGroupRolesSuccessAction act)
        {
            var roles = state.GroupRoles;
            roles = roles.Where(r => !act.RoleIds.Contains(r.Value.Id)).ToList();
            return state with
            {
                GroupRoles = roles,
                Count = roles.Count(),
                IsLoading = false
            };
        }

        [ReducerMethod]
        public static GroupRolesState ReduceToggleAllGroupRolesAction(GroupRolesState state, ToggleAllGroupRolesAction act)
        {
            var roles = state.GroupRoles;
            foreach (var role in roles)
                role.IsSelected = act.IsSelected;

            return state with
            {
                GroupRoles = roles
            };
        }

        [ReducerMethod]
        public static GroupRolesState ReduceToggleGroupRoleAction(GroupRolesState state, ToggleGroupRoleAction act)
        {
            var roles = state.GroupRoles;
            var role = roles.Single(r => r.Value.Id == act.Id);
            role.IsSelected = act.IsSelected;
            return state with
            {
                GroupRoles = roles
            };
        }

        [ReducerMethod]
        public static GroupRolesState ReduceSearchScopesAction(GroupRolesState state, SearchScopesAction act) => state with
        {
            IsEditableRolesLoading = true
        };

        [ReducerMethod]
        public static GroupRolesState ReduceSearchScopesSuccessAction(GroupRolesState state, SearchScopesSuccessAction act)
        {
            if (state.GroupRoles == null) return state;
            var result = act.Scopes.OrderBy(s => s.Name).Select(s => new EditableGroupScope(s)
            {
                IsPresent = state.GroupRoles.Any(sc => sc.Value.Name == s.Name)
            }).ToList();
            return state with
            {
                EditableGroupRoles = result,
                EditableGroupCount = result.Count,
                IsEditableRolesLoading = false
            };
        }

        [ReducerMethod]
        public static GroupRolesState ReduceAddGroupRolesAction(GroupRolesState state, AddGroupRolesAction act) 
        {
            return state with
            {
                IsLoading = false
            };
        }

        [ReducerMethod]
        public static GroupRolesState ReduceAddGroupRolesSuccessAction(GroupRolesState state, AddGroupRolesSuccessAction act)
        {
            var roles = state.GroupRoles.ToList();
            roles.AddRange(act.Roles.Select(r => new SelectableGroupRole(r)
            {
                IsNew = true
            }).ToList());
            return state with
            {
                Count = roles.Count,
                GroupRoles = roles,
                IsLoading = false
            };
        }

        #endregion

        #region GroupHierarchyState

        [ReducerMethod]
        public static GroupHierarchyState ReduceGetHierarchicalGroupAction(GroupHierarchyState state, GetHierarchicalGroupAction act) => new GroupHierarchyState
        {
            IsLoading = true
        };

        [ReducerMethod]
        public static GroupHierarchyState ReduceGetHierarchicalGroupSuccessAction(GroupHierarchyState state, GetHierarchicalGroupSuccessAction act) => new GroupHierarchyState
        {
            IsLoading = false,
            Result = act.Result
        };

        #endregion
    }
}
