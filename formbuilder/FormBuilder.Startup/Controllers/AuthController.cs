// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Repositories;
using FormBuilder.Startup.Controllers.ViewModels;
using FormBuilder.Stores;
using FormBuilder.UIs;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FormBuilder.Startup.Controllers;

public class AuthController : BaseWorkflowController
{
    private const string _workflowId = "sampleWorkflow";
    private const string _stepName = "pwd";

    public AuthController(ITemplateStore templateStore, IAntiforgery antiforgery, IWorkflowStore workflowStore, IFormStore formStore, IOptions<FormBuilderOptions> options) : base(templateStore, antiforgery, workflowStore, formStore, options)
    {
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var viewModel = await BuildViewModel(cancellationToken);
        return View(viewModel);
    }

    private async Task<WorkflowViewModel> BuildViewModel(CancellationToken cancellationToken)
    {
        var viewModel = await Get("master", _workflowId, _stepName, cancellationToken);
        var authViewModel = new AuthViewModel
        {
            // Login = "hello",
            ReturnUrl = "http://localhost:5000",
            ExternalIdProviders = new List<ExternalIdProviderViewModel>
            {
                new ExternalIdProviderViewModel { AuthenticationScheme = "facebook", DisplayName = "Facebook" }
            }
        };
        viewModel.SetInput(authViewModel);
        return viewModel;
    }
}
