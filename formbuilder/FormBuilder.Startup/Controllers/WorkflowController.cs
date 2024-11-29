using FormBuilder.Startup.Controllers.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Startup.Controllers;

public class WorkflowController : Controller
{
    public IActionResult Index()
    {
        var viewModel = new WorkflowIndexViewModel
        {
            Records = new List<Models.FormRecord>
            {
                Constants.LoginPwdAuthForm
            }
        };
        return View(viewModel);
    }
}