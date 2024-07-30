// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.RealmStore
{
    public class RealmReducers
    {
        #region RealmsState

        [ReducerMethod]
        public static RealmsState ReduceGetAllRealmAction(RealmsState state, GetAllRealmAction act) => new(true, new List<Domains.Realm>());

        [ReducerMethod]
        public static RealmsState ReduceGetAllRealmSuccessAction(RealmsState state, GetAllRealmSuccessAction act) => new(false, act.Realms);

        [ReducerMethod]
        public static RealmsState ReduceAddRealmSuccessAction(RealmsState state, AddRealmSuccessAction act)
        {
            var realms = state.Realms.ToList();
            realms.Add(new Domains.Realm
            {
                Name= act.Name,
                Description = act.Description,
                CreateDateTime = DateTime.Now,
                UpdateDateTime = DateTime.Now
            });
            return state with
            {
                Realms = realms
            };
        }

        #endregion

        #region UpdateRealmState

        [ReducerMethod]
        public static UpdateRealmState ReduceAddRealmAction(UpdateRealmState state, AddRealmAction act) => new(true, null);

        [ReducerMethod]
        public static UpdateRealmState ReduceAddRealmSuccessAction(UpdateRealmState state, AddRealmSuccessAction act) => new(false, null);

        [ReducerMethod]
        public static UpdateRealmState ReduceAddRealmFailureAction(UpdateRealmState state, AddRealmFailureAction act) => new(false, act.ErrorMessage);

        #endregion

        #region RealmRolesState

        [ReducerMethod]
        public static RealmRolesState ReduceSearchRealmRolesAction(RealmRolesState state, SearchRealmRolesAction act) => new RealmRolesState(true, new List<RealmRole>());

        [ReducerMethod]
        public static RealmRolesState ReduceSearchRealmRolesSuccessAction(RealmRolesState state, SearchRealmRolesSuccessAction act)
        {
            return state with
            {
                IsLoading = false,
                Count = act.Count,
                RealmRoles = act.RealmRoles.Select(r => new SelectableRealmRole(r))
            };
        }

        [ReducerMethod]
        public static RealmRolesState ReduceToggleRealmRoleAction(RealmRolesState state, ToggleRealmRoleAction act)
        {
            var realmRoles = state.RealmRoles.ToList();
            var realmRole = realmRoles.Single(r => r.Value.Id == act.RealmRoleId);
            realmRole.IsSelected = act.IsSelected;
            return state with
            {
                RealmRoles = realmRoles
            };
        }

        [ReducerMethod]
        public static RealmRolesState ReduceToggleAllRealmRolesAction(RealmRolesState state, ToggleAllRealmRolesAction act)
        {
            var realmRoles = state.RealmRoles.ToList();
            foreach (var realmRole in realmRoles)
                realmRole.IsSelected = act.IsSelected;
            return state with
            {
                RealmRoles = realmRoles
            };
        }

        [ReducerMethod]
        public static RealmRolesState ReduceRemoveSelectedRealmRolesAction(RealmRolesState state, RemoveSelectedRealmRolesAction act)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static RealmRolesState ReduceRemoveSelectedRealmRolesSuccessAction(RealmRolesState state, RemoveSelectedRealmRolesSuccessAction act)
        {
            var result = state.RealmRoles.ToList();
            result = result.Where(v => !act.RealmRoleIds.Contains(v.Value.Id)).ToList();
            return state with
            {
                IsLoading = false,
                Count = result.Count(), 
                RealmRoles = result
            };
        }

        #endregion

        #region RealmRoleState

        [ReducerMethod]
        public static RealmRoleState ReduceGetRealmRoleAction(RealmRoleState state, GetRealmRoleAction act) => new RealmRoleState(true, null);

        [ReducerMethod]
        public static RealmRoleState ReduceGetRealmRoleSuccessAction(RealmRoleState state, GetRealmRoleSuccessAction act) => new RealmRoleState(false, act.RealmRole);

        #endregion
    }
}
