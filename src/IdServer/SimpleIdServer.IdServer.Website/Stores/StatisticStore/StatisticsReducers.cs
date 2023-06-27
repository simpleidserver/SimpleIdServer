// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;

namespace SimpleIdServer.IdServer.Website.Stores.StatisticStore
{
    public class StatisticsReducers
    {
        #region StatisticsState

        [ReducerMethod]
        public static StatisticsState ReduceGetStatisticsAction(StatisticsState state, GetStatisticsAction action) => new StatisticsState { IsLoading = true };

        [ReducerMethod]
        public static StatisticsState ReduceGetStatisticsSuccessAction(StatisticsState state, GetStatisticsSuccessAction action)
        {
            return state with
            {
                IsLoading = false,
                NbClients = action.NbClients,
                NbInvalidAuthentications = action.NbInvalidAuthentications,
                NbUsers = action.NbUsers,
                NbValidAuthentications = action.NbValidAuthentications
            };
        }

        #endregion
    }
}
