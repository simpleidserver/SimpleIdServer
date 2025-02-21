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
    Task<string> GetNextAmr(string realm, string category, string workflowId, string amr, CancellationToken cancellationToken);
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

    public static string GetNextAmr<TViewModel>(WorkflowViewModel result, TViewModel viewModel) where TViewModel : ISidStepViewModel
        => GetNextAmr<TViewModel>(result.Workflow, result.FormRecords, viewModel.CurrentLink);

    public static string GetNextAmr<TViewModel>(WorkflowRecord workflow, List<FormRecord> records, string activeLink)
    {
        var targetFormRecordId = GetTargetFormRecordId(workflow, activeLink);
        if (targetFormRecordId == FormBuilder.Constants.EmptyStep.CorrelationId) return FormBuilder.Constants.EmptyStep.Name;
        var formRecord = records.Single(rec => rec.CorrelationId == targetFormRecordId);
        return formRecord.Name;
    }

    public static bool IsLastStep(string stepName)
        => stepName == FormBuilder.Constants.EmptyStep.Name;

    public async Task<string> GetNextAmr(string realm, string category, string workflowId, string amr, CancellationToken cancellationToken)
    {
        var workflow = await _workflowStore.Get(realm, workflowId, cancellationToken);
        var forms = await _formStore.GetLatestPublishedVersionByCategory(realm, category, cancellationToken);
        return GetNextAmr(workflow, forms, amr);
    }

    public static string GetNextAmr(WorkflowRecord workflow, List<FormRecord> forms, string amr)
    {
        var workflowFormIds = workflow.Steps.Select(r => r.FormRecordCorrelationId);
        var workflowForms = forms.Where(f => workflowFormIds.Contains(f.CorrelationId));
        var currentForm = workflowForms.Single(f => f.Name == amr);
        var workflowStep = workflow.Steps.Single(s => s.FormRecordCorrelationId == currentForm.CorrelationId);
        var stepLinks = workflow.Links.Where(l => l.SourceStepId == workflowStep.Id).Select(l => l.TargetStepId);
        var targetFormIds = workflow.Steps.Where(s => stepLinks.Contains(s.Id)).Select(s => s.FormRecordCorrelationId);
        var result = workflowForms.FirstOrDefault(f => targetFormIds.Contains(f.CorrelationId) && f.ActAsStep)?.Name;
        if (result != null) return result;
        return FormBuilder.Constants.EmptyStep.Name;
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

    private static string GetTargetFormRecordId(WorkflowRecord workflow, string activeLink)
    {
        var link = workflow.Links.Single(l => l.Id == activeLink);
        return workflow.Steps.Single(r => r.Id == link.TargetStepId).FormRecordCorrelationId;
    }
}
