using FormBuilder.Repositories;
using FormBuilder.Startup.Controllers.ViewModels;
using FormBuilder.Stores;
using FormBuilder.UIs;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FormBuilder.Startup.Controllers;

public class RegisterController : BaseWorkflowController
{
    private const string _workflowId = "register";

    public RegisterController(IAntiforgery antiforgery, IWorkflowStore workflowStore, IFormStore formStore, IOptions<FormBuilderOptions> options) : base(antiforgery, workflowStore, formStore, options)
    {
    }

    public async Task<IActionResult> Index(string stepId, CancellationToken cancellationToken)
    {
        var viewModel = await BuildViewModel(stepId, new RegisterViewModel(), cancellationToken);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(RegisterViewModel viewModel, CancellationToken cancellationToken)
    {
        viewModel.IsRegistered = true;
        var result = await BuildViewModel(viewModel.StepId, viewModel, cancellationToken);
        result.SetSuccessMessage("User is registered");
        return View(result);
    }

    [HttpGet]
    public IActionResult Callback(string scheme)
    {
        return NoContent();
    }

    private async Task<WorkflowViewModel> BuildViewModel(string stepName, RegisterViewModel viewModel, CancellationToken cancellationToken)
    {
        var result = await Get(_workflowId, stepName, cancellationToken);
        result.SetInput(viewModel);
        return result;
    }
}
