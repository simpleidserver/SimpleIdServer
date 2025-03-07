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

    [ReducerMethod]
    public static RecurringJobsState ReduceEnableRecurringJobAction(RecurringJobsState state, EnableRecurringJobAction act)
    {
        return state with
        {
            IsLoading = true
        };
    }

    [ReducerMethod]
    public static RecurringJobsState ReduceEnableRecurringJobSuccessAction(RecurringJobsState state, EnableRecurringJobSuccessAction act)
    {
        var recurringJobs = state.RecurringJobs;
        var job = recurringJobs.Single(r => r.Id == act.Id);
        job.IsDisabled = false;
        return state with
        {
            IsLoading = false,
            RecurringJobs = recurringJobs
        };
    }

    [ReducerMethod]
    public static RecurringJobsState ReduceDisableRecurringJobAction(RecurringJobsState state, DisableRecurringJobAction act)
    {
        return state with
        {
            IsLoading = true
        };
    }

    [ReducerMethod]
    public static RecurringJobsState ReduceDisableRecurringJobSuccessAction(RecurringJobsState state, DisableRecurringJobSuccessAction act)
    {
        var recurringJobs = state.RecurringJobs;
        var job = recurringJobs.Single(r => r.Id == act.Id);
        job.IsDisabled = true;
        return state with
        {
            IsLoading = false,
            RecurringJobs = recurringJobs
        };
    }

    #endregion

    #region LastFailedJobsState

    [ReducerMethod]
    public static LastFailedJobsState ReduceGetLastFailedJobsAction(LastFailedJobsState state, GetLastFailedJobsAction act)
    {
        return state with
        {
            IsLoading = true
        };
    }

    [ReducerMethod]
    public static LastFailedJobsState ReduceGetLastFailedJobsSuccessAction(LastFailedJobsState state, GetLastFailedJobsSuccessAction act)
    {
        return state with
        {
            IsLoading = false,
            FailedJobs = act.FailedJobs
        };
    }

    #endregion
}