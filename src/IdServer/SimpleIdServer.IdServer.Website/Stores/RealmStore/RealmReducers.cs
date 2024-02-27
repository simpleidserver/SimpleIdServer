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
        public static RealmsState ReduceSelectRealmAction(RealmsState state, SelectRealmAction act)
        {
            return state with
            {
                ActiveRealm = null,
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static RealmsState ReduceGetActiveSuccessRealmAction(RealmsState state, GetActiveSuccessRealmAction act)
        {
            return state with
            {
                IsLoading = false,
                ActiveRealm = act.Realm
            };
        }

        [ReducerMethod]
        public static RealmsState ReduceSelectRealmSuccessAction(RealmsState state, SelectRealmSuccessAction act)
        {
            return state with
            {
                IsLoading = false,
                ActiveRealm = act.Realm
            };
        }

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
                Realms = realms,
                ActiveRealm = act.Name
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
    }
}
