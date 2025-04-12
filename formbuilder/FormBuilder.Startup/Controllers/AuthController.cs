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

    public AuthController(IAntiforgery antiforgery, IWorkflowStore workflowStore, IFormStore formStore, IOptions<FormBuilderOptions> options) : base(antiforgery, workflowStore, formStore, options)
    {
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var viewModel = await BuildViewModel(cancellationToken);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(AuthViewModel viewModel, CancellationToken cancellationToken)
    {
        if(string.IsNullOrWhiteSpace(viewModel.Login))
        {
            var vm = await BuildViewModel(cancellationToken);
            vm.ErrorMessages = new List<string>
            {
                "The login is required"
            };
            return View(vm);
        }

        //var result = await GetNextWorkflowStep(cancellationToken);
        //if (result == null) return Content("finish");
        //return RedirectToAction("Index", new { workflowId = _workflowId, stepName = result.Value.Item2.FormRecordId });
        return null;
    }

    [HttpGet]
    public IActionResult Callback(string scheme)
    {
        return NoContent();
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
