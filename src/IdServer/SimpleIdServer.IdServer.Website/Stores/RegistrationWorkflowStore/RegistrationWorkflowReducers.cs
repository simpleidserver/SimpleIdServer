// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;
using SimpleIdServer.IdServer.Api.RegistrationWorkflows;

namespace SimpleIdServer.IdServer.Website.Stores.RegistrationWorkflowStore;

public class RegistrationWorkflowReducers
{
    #region RegistrationWorkflowsState

    [ReducerMethod]
    public static RegistrationWorkflowsState ReduceUpdateRegistrationWorkflowAction(RegistrationWorkflowsState state, UpdateRegistrationWorkflowAction act)
    {
        return state with
        {
            IsLoading = true
        };
    }

    [ReducerMethod]
    public static RegistrationWorkflowsState ReduceUpdateRegistrationWorkflowSuccessAction(RegistrationWorkflowsState state, UpdateRegistrationWorkflowSuccessAction act)
    {
        var registrationWorkflows = state.RegistrationWorkflows.ToList();
        var defaultRegistrationWorkflows = registrationWorkflows.Where(r => r.RegistrationWorkflow.IsDefault);
        foreach (var defaultRegistrationWorkflow in defaultRegistrationWorkflows) defaultRegistrationWorkflow.RegistrationWorkflow.IsDefault = false;
        var selectedRegistrationWorkflow = registrationWorkflows.Single(w => w.RegistrationWorkflow.Id == act.Id);
        selectedRegistrationWorkflow.RegistrationWorkflow.IsDefault = act.IsDefault;
        return state with
        {
            IsLoading = false,
            RegistrationWorkflows = registrationWorkflows
        };
    }

    [ReducerMethod]
    public static RegistrationWorkflowsState ReduceUpdateRegistrationWorkflowFailureAction(RegistrationWorkflowsState state, UpdateRegistrationWorkflowFailureAction act)
    {
        return state with
        {
            IsLoading = false
        };
    }


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
            RegistrationWorkflows = registrationWorkflows,
            Count = registrationWorkflows.Count
        };
    }

    [ReducerMethod]
    public static RegistrationWorkflowsState ReduceAddRegistrationWorkflowAction(RegistrationWorkflowsState state, AddRegistrationWorkflowAction action)
    {
        return state with
        {
            IsLoading = true
        };
    }

    [ReducerMethod]
    public static RegistrationWorkflowsState ReduceAddRegistrationWorkflowSuccessAction(RegistrationWorkflowsState state, AddRegistrationWorkflowSuccessAction action)
    {
        var registrationWorkflows = state.RegistrationWorkflows.ToList();
        if(action.IsDefault)
            registrationWorkflows.ForEach(r => r.RegistrationWorkflow.IsDefault = false);
        registrationWorkflows.Add(new SelectableRegistrationWorkflow(new RegistrationWorkflowResult { CreateDateTime = DateTime.Now, Id = action.Id, IsDefault = action.IsDefault, Name = action.Name, UpdateDateTime = DateTime.Now }, false)
        {
            IsNew = true
        });
        return state with
        {
            RegistrationWorkflows = registrationWorkflows,
            Count = registrationWorkflows.Count,
            IsLoading = false
        };
    }

    [ReducerMethod]
    public static RegistrationWorkflowsState ReduceAddRegistrationWorkflowFailureAction(RegistrationWorkflowsState state, AddRegistrationWorkflowFailureAction action)
    {
        return state with
        {
            IsLoading = false
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
        registrationWorkflow.UpdateDateTime = DateTime.Now;
        registrationWorkflow.IsDefault = act.IsDefault;
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

    #region RegistrationFormsState

    [ReducerMethod]
    public static RegistrationFormsState ReduceGetAllRegistrationFormsAction(RegistrationFormsState state, GetAllRegistrationFormsAction action) => new(true);

    [ReducerMethod]
    public static RegistrationFormsState ReduceGetAllRegistrationFormsSuccessAction(RegistrationFormsState state, GetAllRegistrationFormsSuccessAction action)
    {
        return state with
        {
            IsLoading = false,
            FormRecords = action.RegistrationForms
        };
    }
    #endregion
}