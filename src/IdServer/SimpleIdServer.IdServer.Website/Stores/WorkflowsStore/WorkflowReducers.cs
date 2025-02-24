// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;
using SimpleIdServer.IdServer.Website.Stores.AcrsStore;
using SimpleIdServer.IdServer.Website.Stores.RegistrationWorkflowStore;

namespace SimpleIdServer.IdServer.Website.Stores.WorkflowsStore;

public class WorkflowReducers
{
    #region WorkflowState

    [ReducerMethod]
    public static WorkflowState ReduceGetWorkflowAction(WorkflowState state, GetWorkflowAction action)
    {
        return state with
        {
            IsLoading = true
        };
    }

    [ReducerMethod]
    public static WorkflowState ReduceUpdateWorkflowAction(WorkflowState state, UpdateWorkflowAction action)
    {
        return state with
        {
            IsLoading = true
        };
    }

    [ReducerMethod]
    public static WorkflowState ReduceGetWorkflowSuccessAction(WorkflowState state, GetWorkflowSuccessAction action)
    {
        return state with
        {
            IsLoading = false,
            Value = action.Workflow
        };
    }

    [ReducerMethod]
    public static WorkflowState ReduceUpdateWorkflowSuccessAction(WorkflowState state, UpdateWorkflowSuccessAction action)
    {
        return state with
        {
            IsLoading = false
        };
    }

    #endregion

    #region WorkflowLayoutListState

    [ReducerMethod]
    public static WorkflowLayoutListState ReduceGetAllAuthenticationWorkflowLayoutsAction(WorkflowLayoutListState state, GetAllAuthenticationWorkflowLayoutsAction action) => new WorkflowLayoutListState
    {
        IsLoading = true
    };

    [ReducerMethod]
    public static WorkflowLayoutListState ReduceGetAllAuthenticationWorkflowLayoutsSuccessAction(WorkflowLayoutListState state, GetAllAuthenticationWorkflowLayoutsSuccessAction action)
    {
        return state with
        {
            IsLoading = false,
            Values = action.WorkflowLayouts
        };
    }

    [ReducerMethod]
    public static WorkflowLayoutListState ReduceGetAllRegistrationWorkflowLayoutsAction(WorkflowLayoutListState state, GetAllRegistrationWorkflowLayoutsAction action) => new WorkflowLayoutListState
    {
        IsLoading = true
    };

    [ReducerMethod]
    public static WorkflowLayoutListState ReduceGetAllRegistrationWorkflowLayoutsSuccessAction(WorkflowLayoutListState state, GetAllRegistrationWorkflowLayoutsSuccessAction action)
    {
        return state with
        {
            IsLoading = false,
            Values = action.WorkflowLayouts
        };
    }

    #endregion
}
