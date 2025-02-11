using FormBuilder.Startup.Controllers.ViewModels;
using FormBuilder.Startup.Workflows;
using FormBuilder.Stores;
using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Startup.Controllers;

public class EditorController : Controller
{
    private readonly IFormStore _formStore;

    public EditorController(IFormStore formStore)
    {
        _formStore = formStore;
    }

    public async Task<IActionResult> Index()
    {
        var workflowLayout = new PwdAuthWorkflowLayout().Get();
        var form = await _formStore.GetLatestVersionByCorrelationId(Constants.DefaultRealm, PwdAuthForms.LoginPwdAuthForm.CorrelationId, CancellationToken.None);
        var viewModel = new IndexEditorViewModel
        {
            Record = form,
            WorkflowLayout = workflowLayout
        };
        return View(viewModel);
    }
}