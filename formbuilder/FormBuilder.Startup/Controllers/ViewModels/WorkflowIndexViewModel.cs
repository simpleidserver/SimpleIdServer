using FormBuilder.Models;
using FormBuilder.Models.Layout;

namespace FormBuilder.Startup.Controllers.ViewModels;

public class WorkflowIndexViewModel
{
    public List<FormRecord> Records { get; set; }
    public List<WorkflowLayout> WorkflowLayouts { get; set; }
}
