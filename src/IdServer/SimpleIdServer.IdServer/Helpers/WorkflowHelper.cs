// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Models;
using FormBuilder.Repositories;
using FormBuilder.Stores;
using FormBuilder.UIs;
using SimpleIdServer.IdServer.UI.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Helpers;

public interface IWorkflowHelper
{
    Task<string> GetNextAmr(string workflowId, string amr, CancellationToken cancellationToken);
}

public class WorkflowHelper : IWorkflowHelper
{
    private readonly IFormStore _formStore;
    private readonly IWorkflowStore _workflowStore;

    public WorkflowHelper(IFormStore formStore, IWorkflowStore workflowStore)
    {
        _formStore = formStore;
        _workflowStore = workflowStore;
    }

    public static string GetNextAmr<TViewModel>(WorkflowViewModel result, TViewModel viewModel) where TViewModel : IRegisterViewModel
        => GetNextAmr<TViewModel>(result.Workflow, result.FormRecords, viewModel.CurrentLink);

    public static string GetNextAmr<TViewModel>(WorkflowRecord workflow, List<FormRecord> records, string activeLink)
    {
        var targetFormRecordId = GetTargetFormRecordId(workflow, activeLink);
        var formRecord = records.Single(rec => rec.Id == targetFormRecordId);
        return formRecord.Name;
    }

    public static bool IsLastStep(string stepName)
        => stepName == FormBuilder.Constants.EmptyStep.Name;

    public async Task<string> GetNextAmr(string workflowId, string amr, CancellationToken cancellationToken)
    {
        var workflow = await _workflowStore.Get(workflowId, cancellationToken);
        var workflowFormIds = workflow.Steps.Select(r => r.FormRecordId);
        var forms = await _formStore.GetAll(cancellationToken);
        var workflowForms = forms.Where(f => workflowFormIds.Contains(f.Id));
        var currentForm = workflowForms.Single(f => f.Name == amr);
        var workflowStep = workflow.Steps.Single(s => s.FormRecordId == currentForm.Id);
        var link = workflow.Links.Where(l => l.SourceStepId == workflowStep.Id).First();
        var targetStep = workflow.Steps.Single(s => s.Id == link.TargetStepId);
        return workflowForms.Single(f => f.Id == targetStep.FormRecordId).Name;
    }

    private static string GetTargetFormRecordId(WorkflowRecord workflow, string activeLink)
    {
        var link = workflow.Links.Single(l => l.Id == activeLink);
        return workflow.Steps.Single(r => r.Id == link.TargetStepId).FormRecordId;
    }
}
