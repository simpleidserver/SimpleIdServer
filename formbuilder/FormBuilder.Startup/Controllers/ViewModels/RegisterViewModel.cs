using FormBuilder.UIs;

namespace FormBuilder.Startup.Controllers.ViewModels;

public class RegisterViewModel : IStepViewModel
{
    public bool IsRegistered { get; set; }
    public string StepId { get; set; }
    public string WorkflowId { get; set; }
    public string CurrentLink { get; set; }
}
