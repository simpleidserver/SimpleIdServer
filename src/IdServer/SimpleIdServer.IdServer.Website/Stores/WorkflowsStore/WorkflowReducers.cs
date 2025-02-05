// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;

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
    public static WorkflowState ReducePublishWorkflowAction(WorkflowState state, PublishWorkflowAction action)
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

    [ReducerMethod]
    public static WorkflowState ReducePublishWorkflowSuccessAction(WorkflowState state, PublishWorkflowSuccessAction action)
    {
        return state with
        {
            IsLoading = false,
            Value = action.Workflow
        };
    }

    #endregion
}
