using FormBuilder.Models;

namespace FormBuilder.Components.FormElements.StackLayout;

public class FormStackLayoutRecord : BaseFormLayoutRecord
{
    public bool IsFormEnabled { get; set; } = false;
    public ITargetUrl Url { get; set; }
}