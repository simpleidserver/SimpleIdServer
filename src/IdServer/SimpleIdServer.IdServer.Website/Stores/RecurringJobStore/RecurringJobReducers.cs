// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;

namespace SimpleIdServer.IdServer.Website.Stores.RecurringJobStore;

public class RecurringJobReducers
{
    #region RecurringJobsState

    [ReducerMethod]
    public static RecurringJobsState ReduceGetRecurringJobsAction(RecurringJobsState state, GetRecurringJobsAction act) => new RecurringJobsState { IsLoading = true };

    [ReducerMethod]
    public static RecurringJobsState ReduceGetRecurringJobsSuccessAction(RecurringJobsState state, GetRecurringJobsSuccessAction act) => new RecurringJobsState { IsLoading = false, RecurringJobs = act.RecurringJobs };

    [ReducerMethod]
    public static RecurringJobsState ReduceUpdateRecurringJobAction(RecurringJobsState state, UpdateRecurringJobAction act)
    {
        return state with
        {
            IsLoading = true
        };
    }

    [ReducerMethod]
    public static RecurringJobsState ReduceUpdateRecurringJobAction(RecurringJobsState state, UpdateRecurringJobSuccessAction act)
    {
        var recurringJobs = state.RecurringJobs;
        var recurringJob = recurringJobs.Single(r => r.Id == act.Id);
        recurringJob.Cron = act.Cron;
        return state with
        {
            IsLoading = false,
            RecurringJobs = recurringJobs
        };
    }

    #endregion

    #region RecurringJobState

    [ReducerMethod]
    public static RecurringJobState ReduceGetRecurringJobAction(RecurringJobState state, GetRecurringJobAction act)
    {
        return state with
        {
            IsLoading = true
        };
    }

    [ReducerMethod]
    public static RecurringJobState ReduceGetRecurringJobSuccessAction(RecurringJobState state, GetRecurringJobSuccessAction act)
    {
        return state with
        {
            IsLoading = false,
            Id = act.Id,
            Histories = act.Histories
        };
    }

    #endregion

    #region HangfireServersState

    [ReducerMethod]
    public static HangfireServersState ReduceGetServersAction(HangfireServersState state, GetServersAction act)
    {
        return state with
        {
            IsLoading = true
        };
    }

    [ReducerMethod]
    public static HangfireServersState ReduceGetServersSuccessAction(HangfireServersState state, GetServersSuccessAction act)
    {
        return state with
        {
            IsLoading = false,
            Servers = act.Servers
        };
    }

    #endregion
}