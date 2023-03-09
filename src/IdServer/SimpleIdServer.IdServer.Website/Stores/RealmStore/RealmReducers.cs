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
        public static RealmsState ReduceGetAllRealmAction(RealmsState state, GetAllRealmAction act) => new(true, new List<Realm>());

        [ReducerMethod]
        public static RealmsState ReduceGetAllRealmSuccessAction(RealmsState state, GetAllRealmSuccessAction act) => new(false, act.Realms);

        [ReducerMethod]
        public static RealmsState ReduceSelectRealmAction(RealmsState state, SelectRealmAction act)
        {
            return state with
            {
                ActiveRealm = act.Realm
            };
        }

        #endregion
    }
}
