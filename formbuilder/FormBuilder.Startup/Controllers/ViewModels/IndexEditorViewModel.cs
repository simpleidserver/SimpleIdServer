using FormBuilder.Models;
using FormBuilder.Models.Layout;

namespace FormBuilder.Startup.Controllers.ViewModels;

public class IndexEditorViewModel
{
    public FormRecord Record { get; set; }
    public WorkflowLayout WorkflowLayout { get; set; }
}
