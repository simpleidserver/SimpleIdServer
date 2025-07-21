// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Helpers;
using FormBuilder.Models;
using FormBuilder.Repositories;
using FormBuilder.Stores;
using FormBuilder.UIs;
using SimpleIdServer.IdServer.UI.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Helpers;

public interface IWorkflowHelper
{
    string GetNextAmr<TViewModel>(JsonObject input, WorkflowViewModel result, TViewModel viewModel) where TViewModel : ISidStepViewModel;
    string GetNextAmr<TViewModel>(JsonObject input, WorkflowRecord workflow, List<FormRecord> records, string activeLink);
    Task<string> GetNextAmr(JsonObject input, string realm, string category, string workflowId, string amr, CancellationToken cancellationToken);
    string GetNextAmr(JsonObject input, WorkflowRecord workflow, List<FormRecord> forms, string amr);
    string GetTargetFormRecordId<TViewModel>(JsonObject input, WorkflowRecord workflow, TViewModel viewModel) where TViewModel : ISidStepViewModel;
}

public class WorkflowHelper : IWorkflowHelper
{
    private readonly IFormStore _formStore;
    private readonly IWorkflowStore _workflowStore;
    private readonly IWorkflowLinkHelper _workflowLinkHelper;

    public WorkflowHelper(
        IFormStore formStore, 
        IWorkflowStore workflowStore,
        IWorkflowLinkHelper workflowLinkHelper)
    {
        _formStore = formStore;
        _workflowStore = workflowStore;
        _workflowLinkHelper = workflowLinkHelper;
    }

    public string GetNextAmr<TViewModel>(JsonObject input, WorkflowViewModel result, TViewModel viewModel) where TViewModel : ISidStepViewModel
        => GetNextAmr<TViewModel>(input, result.Workflow, result.FormRecords, viewModel.CurrentLink);

    public string GetNextAmr<TViewModel>(JsonObject input, WorkflowRecord workflow, List<FormRecord> records, string activeLink)
    {
        var targetFormRecordId = GetTargetFormRecordId(input, workflow, activeLink).FormRecordCorrelationId;
        if (targetFormRecordId == FormBuilder.Constants.EmptyStep.CorrelationId) return FormBuilder.Constants.EmptyStep.Name;
        var formRecord = records.Single(rec => rec.CorrelationId == targetFormRecordId);
        return formRecord.Name;
    }

    public async Task<string> GetNextAmr(JsonObject input, string realm, string category, string workflowId, string amr, CancellationToken cancellationToken)
    {
        var workflow = await _workflowStore.Get(realm, workflowId, cancellationToken);
        var forms = await _formStore.GetLatestPublishedVersionByCategory(realm, category, cancellationToken);
        return GetNextAmr(input, workflow, forms, amr);
    }

    public string GetNextAmr(JsonObject input, WorkflowRecord workflow, List<FormRecord> forms, string amr)
    {
        var workflowFormIds = workflow.Steps.Select(r => r.FormRecordCorrelationId);
        var workflowForms = forms.Where(f => workflowFormIds.Contains(f.CorrelationId));
        var currentForm = workflowForms.Single(f => f.Name == amr);
        var workflowStep = workflow.Steps.Single(s => s.FormRecordCorrelationId == currentForm.CorrelationId);
        var stepLinks = workflow.Links.Where(l => l.SourceStepId == workflowStep.Id).Select(s => _workflowLinkHelper.ResolveNextStep(input, workflow, s.Id));
        var targetFormIds = workflow.Steps.Where(s => stepLinks.Contains(s.Id)).Select(s => s.FormRecordCorrelationId);
        var result = workflowForms.FirstOrDefault(f => targetFormIds.Contains(f.CorrelationId) && f.ActAsStep)?.Name;
        if (result != null) return result;
        return FormBuilder.Constants.EmptyStep.Name;
    }

    public string GetTargetFormRecordId<TViewModel>(JsonObject input, WorkflowRecord workflow, TViewModel viewModel) 
        where TViewModel : ISidStepViewModel
    {
        return GetTargetFormRecordId(input, workflow, viewModel.CurrentLink).Id;
    }

    public static List<string> ExtractAmrs(WorkflowRecord workflow, List<FormRecord> forms)
    {
        var rootStep = workflow.GetFirstStep();
        var formCorrelationIds = new List<string>
        {
          rootStep.FormRecordCorrelationId
        };
        formCorrelationIds.AddRange(workflow.GetAllChildrenMainLinks(rootStep.Id).Select(r => r.FormRecordCorrelationId));
        var names = formCorrelationIds.Select(id => forms.Single(f => f.CorrelationId == id)).Where(f => f.ActAsStep).Select(f => f.Name).ToList();
        return names;
    }

    public static bool IsLastStep(string stepName)
        => stepName == FormBuilder.Constants.EmptyStep.Name;

    private WorkflowStep GetTargetFormRecordId(JsonObject input, WorkflowRecord workflow, string activeLink)
    {
        var stepId = _workflowLinkHelper.ResolveNextStep(input, workflow, activeLink);
        return workflow.Steps.Single(s => s.Id == stepId);
    }
}
