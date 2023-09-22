// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;
using SimpleIdServer.IdServer.Api.RegistrationWorkflows;

namespace SimpleIdServer.IdServer.Website.Stores.RegistrationWorkflowStore;

public class RegistrationWorkflowReducers
{
    #region RegistrationWorkflowsState

    [ReducerMethod]
    public static RegistrationWorkflowsState ReduceGetAllRegistrationWorkflowsAction(RegistrationWorkflowsState state, GetAllRegistrationWorkflowsAction act) => new RegistrationWorkflowsState(true, new List<RegistrationWorkflowResult>());

    [ReducerMethod]
    public static RegistrationWorkflowsState ReduceGetAllRegistrationWorkflowsSuccessAction(RegistrationWorkflowsState state, GetAllRegistrationWorkflowsSuccessAction act) => new RegistrationWorkflowsState(false, act.RegistrationWorkflows);

    [ReducerMethod]
    public static RegistrationWorkflowsState ReduceToggleRegistrationWorkflowAction(RegistrationWorkflowsState state, ToggleRegistrationWorkflowAction act)
    {
        var registrationWorkflows = state.RegistrationWorkflows;
        var selectedRegistrationWorkflow = registrationWorkflows.Single(w => w.RegistrationWorkflow.Id == act.Id);
        selectedRegistrationWorkflow.IsSelected = act.IsSelected;
        return state with
        {
            RegistrationWorkflows = registrationWorkflows
        };
    }

    [ReducerMethod]
    public static RegistrationWorkflowsState ReduceToggleAllRegistrationWorkflowAction(RegistrationWorkflowsState state, ToggleAllRegistrationWorkflowAction act)
    {
        var registrationWorkflows = state.RegistrationWorkflows;
        foreach (var registrationWorkflow in registrationWorkflows) registrationWorkflow.IsSelected = act.IsSelected;
        return state with
        {
            RegistrationWorkflows = registrationWorkflows
        };
    }

    [ReducerMethod]
    public static RegistrationWorkflowsState ReduceRemoveSelectedRegistrationWorkflowsAction(RegistrationWorkflowsState state, RemoveSelectedRegistrationWorkflowsAction act)
    {
        return state with
        {
            IsLoading = true
        };
    }

    [ReducerMethod]
    public static RegistrationWorkflowsState ReduceRemoveSelectedRegistrationWorkflowsSuccessAction(RegistrationWorkflowsState state, RemoveSelectedRegistrationWorkflowsSuccessAction act)
    {
        var registrationWorkflows = state.RegistrationWorkflows.Where(w => !act.Ids.Contains(w.RegistrationWorkflow.Id)).ToList();
        return state with
        {
            IsLoading = false,
            RegistrationWorkflows = registrationWorkflows
        };
    }

    #endregion

    #region RegistrationWorkflowState

    [ReducerMethod]
    public static RegistrationWorkflowState ReduceGetRegistrationWorkflowAction(RegistrationWorkflowState state, GetRegistrationWorkflowAction act) => new RegistrationWorkflowState(null, true);

    [ReducerMethod]
    public static RegistrationWorkflowState ReduceGetRegistrationWorkflowSuccessAction(RegistrationWorkflowState state, GetRegistrationWorkflowSuccessAction act) => new RegistrationWorkflowState(act.RegistrationWorkflow, false);

    [ReducerMethod]
    public static RegistrationWorkflowState ReduceUpdateRegistrationWorkflowAction(RegistrationWorkflowState state, UpdateRegistrationWorkflowAction act)
    {
        return state with
        {
            IsLoading = true
        };
    }

    [ReducerMethod]
    public static RegistrationWorkflowState ReduceUpdateRegistrationWorkflowSuccessAction(RegistrationWorkflowState state, UpdateRegistrationWorkflowSuccessAction act)
    {
        var registrationWorkflow = state.Value;
        registrationWorkflow.UpdateDateTime = DateTime.UtcNow;
        registrationWorkflow.IsDefault = act.IsDefault;
        registrationWorkflow.Steps = act.Steps;
        return state with
        {
            IsLoading = false,
            Value = registrationWorkflow
        };
    }

    [ReducerMethod]
    public static RegistrationWorkflowState ReduceUpdateRegistrationWorkflowFailureAction(RegistrationWorkflowState state, UpdateRegistrationWorkflowFailureAction act)
    {
        return state with
        {
            IsLoading = false
        };
    }

    #endregion
}