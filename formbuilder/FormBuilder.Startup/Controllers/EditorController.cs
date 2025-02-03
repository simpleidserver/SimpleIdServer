using FormBuilder.Startup.Controllers.ViewModels;
using FormBuilder.Startup.Workflows;
using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Startup.Controllers;

public class EditorController : Controller
{
    public IActionResult Index()
    {
        var viewModel = new IndexEditorViewModel
        {
            Record = PwdAuthForms.LoginPwdAuthForm
        };
        return View(viewModel);
    }
}