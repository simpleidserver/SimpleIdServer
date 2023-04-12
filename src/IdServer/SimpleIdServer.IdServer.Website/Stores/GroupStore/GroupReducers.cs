// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

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
            if (!string.IsNullOrWhiteSpace(act.ParentGroupId)) return state;
            var groups = state.Groups.ToList();
            groups.Add(new SelectableGroup(new Group { CreateDateTime = DateTime.UtcNow, UpdateDateTime = DateTime.UtcNow, Id = act.Id, Name = act.Name, Description = act.Description })
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
            if (state.Groups == null) return state;
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

        #endregion

        #region GroupState

        [ReducerMethod]
        public static GroupState ReduceGetGroupAction(GroupState state, GetGroupAction act) => new(true, null);

        [ReducerMethod]
        public static GroupState ReduceGetGroupSuccessAction(GroupState state, GetGroupSuccessAction act) => new(false, act.Group);

        [ReducerMethod]
        public static GroupState ReduceGetGroupFailureAction(GroupState state, GetGroupFailureAction act) => new(false, null);

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
            if (string.IsNullOrWhiteSpace(action.ParentId)) return state;
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
            if (string.IsNullOrWhiteSpace(action.ParentGroupId)) return state;
            var members = state.Members.ToList();
            members.Add(new SelectableGroupMember(new Group { Id = action.Id, Name = action.Name, ParentGroupId = action.ParentGroupId, Description = action.Description, CreateDateTime = DateTime.UtcNow, UpdateDateTime = DateTime.UtcNow })
            {
                IsNew = true
            });
            return state with
            {
                Members = members,
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
    }
}
