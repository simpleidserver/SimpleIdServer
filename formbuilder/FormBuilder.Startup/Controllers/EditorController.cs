using FormBuilder.Startup.Controllers.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Startup.Controllers;

public class EditorController : Controller
{
    public IActionResult Index()
    {
        var viewModel = new IndexEditorViewModel
        {
            Record = Constants.LoginPwdAuthForm
        };
        return View(viewModel);
    }
}
