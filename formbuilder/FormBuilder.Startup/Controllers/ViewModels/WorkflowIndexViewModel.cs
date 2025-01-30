using FormBuilder.Models;
using FormBuilder.Models.Layout;

namespace FormBuilder.Startup.Controllers.ViewModels;

public class WorkflowIndexViewModel
{
    public List<FormRecord> Forms { get; set; }
    public List<WorkflowLayout> WorkflowLayouts { get; set; }
    public WorkflowRecord Workflow { get; set; }
}