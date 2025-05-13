// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;

namespace SimpleIdServer.IdServer.Website.Stores.MigrationStore;

public static class MigrationExecutionsReducers
{
    #region MigrationExecutionsState

    [ReducerMethod]
    public static MigrationExecutionsState ReduceGetAllMigrationsExecutionsAction(MigrationExecutionsState state, GetAllMigrationsExecutionsAction act) => new MigrationExecutionsState();

    [ReducerMethod]
    public static MigrationExecutionsState ReduceGetAllMigrationsExecutionsSuccessAction(MigrationExecutionsState state, GetAllMigrationsExecutionsSuccessAction act)
    {
        return state with
        {
            IsLoading = false,
            Executions = act.Executions
        };
    }

    [ReducerMethod]
    public static MigrationExecutionsState ReduceLaunchMigrationAction(MigrationExecutionsState state, LaunchMigrationAction act)
    {
        return state with
        {
            IsLoading = false
        };
    }

    [ReducerMethod]
    public static MigrationExecutionsState ReduceLaunchMigrationSuccessAction(MigrationExecutionsState state, LaunchMigrationSuccessAction act)
    {
        return state with
        {
            IsLoading = false
        };
    }

    #endregion
}
